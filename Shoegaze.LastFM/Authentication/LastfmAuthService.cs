using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Shoegaze.LastFM.Authentication;

/// <summary>
/// Handles OAuth 1.0a authentication with the Last.fm API.
/// </summary>
public class LastfmAuthService(HttpClient httpClient, string apiKey, string apiSecret, string callbackUrl) : IAuthService
{
  private const string SessionUrl = "https://ws.audioscrobbler.com/2.0/";
  private static readonly CompositeFormat UnformatedAuthUrl = CompositeFormat.Parse("https://www.last.fm/api/auth/?api_key={0}&cb={1}");

  private readonly string _apiKey = apiKey;
  private readonly string _apiSecret = apiSecret;
  private readonly string _callbackUrl = callbackUrl;
  private readonly HttpClient _http = httpClient;

  /// <summary>
  /// Generates the authorization URL that the user must visit to grant access.
  /// </summary>
  /// <returns>
  /// A <see cref="Uri"/> pointing to the Last.fm authentication page with the
  /// configured API key and callback URL.
  /// </returns>
  public Task<Uri> GetAuthorizationUrlAsync()
  {
    var encodedCallback = HttpUtility.UrlEncode(_callbackUrl);
    var authUrl = string.Format(CultureInfo.InvariantCulture, UnformatedAuthUrl, _apiKey, encodedCallback);
    return Task.FromResult(new Uri(authUrl));
  }

  /// <summary>
  /// Exchanges a temporary token for a permanent Last.fm session.
  /// </summary>
  /// <param name="token">The request token received from Last.fm after user authorization.</param>
  /// <param name="tokenSecret">Unused in Last.fm’s implementation of OAuth 1.0a, can be passed as an empty string.</param>
  /// <param name="verifier">Unused in Last.fm’s implementation of OAuth 1.0a, can be passed as an empty string.</param>
  /// <returns>
  /// An <see cref="AuthSession"/> containing the authorized user’s name and session key.
  /// </returns>
  /// <exception cref="HttpRequestException">Thrown if the HTTP request fails or Last.fm returns a non-success status code.</exception>
  /// <exception cref="JsonException">Thrown if the response cannot be parsed as valid JSON.</exception>
  public async Task<AuthSession> GetSessionAsync(string token, string tokenSecret, string verifier)
  {
    var parameters = new Dictionary<string, string>
        {
            { "method", "auth.getSession" },
            { "api_key", _apiKey },
            { "token", token },
        };

    // Sign with API secret
    var apiSig = GenerateApiSignature(parameters, _apiSecret);
    parameters["api_sig"] = apiSig;
    parameters["format"] = "json";

    using var content = new FormUrlEncodedContent(parameters);
    using var request = new HttpRequestMessage(HttpMethod.Post, SessionUrl)
    {
      Content = content
    };
    using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
    response.EnsureSuccessStatusCode();

    await using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    using var doc = await JsonDocument.ParseAsync(stream).ConfigureAwait(false);

    var session = doc.RootElement.GetProperty("session");
    return new AuthSession(session.GetProperty("name").GetString()!, session.GetProperty("key").GetString()!);
  }

  internal static string GenerateApiSignature(IDictionary<string, string> parameters, string secret)
  {
    var filtered = parameters
      .Where(kv => kv.Key != "format" && kv.Key != "callback" && kv.Key != "api_sig")
      .OrderBy(kv => kv.Key, StringComparer.Ordinal);

    var builder = new StringBuilder();
    foreach (var kv in filtered)
      builder.Append(kv.Key).Append(kv.Value);

    builder.Append(secret);
    return CalculateMd5(builder.ToString());
  }

#pragma warning disable IDE0079 // Remove unnecessary suppression
  [SuppressMessage("Security", "CA5351:Do Not Use Broken Cryptographic Algorithms", Justification = "Last.fm API requires MD5 for API signature generation.")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
  private static string CalculateMd5(string input)
  {
    var inputBytes = Encoding.UTF8.GetBytes(input);
    var hashBytes = MD5.HashData(inputBytes);
    return Convert.ToHexStringLower(hashBytes);
  }

  /// <summary>
  /// Starts the full authentication process, including opening the system browser,
  /// listening for the Last.fm callback, and exchanging the temporary token for a session key.
  /// </summary>
  /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
  /// <returns>
  /// An <see cref="AuthSession"/> containing the authorized user’s name and session key.
  /// </returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown if the system browser cannot be opened or if Last.fm returns no token.
  /// </exception>
  /// <exception cref="TimeoutException">
  /// Thrown if the user does not complete authentication within 5 minutes.
  /// </exception>
  /// <remarks>
  /// This method launches the user’s default browser and starts a temporary HTTP listener
  /// on <c>http://localhost:{port}/</c> to capture the Last.fm callback.
  /// </remarks>
  public async Task<AuthSession> AuthenticateAsync(CancellationToken cancellationToken = default)
  {
    var port = GetFreePort();
    var callbackUri = new Uri($"http://localhost:{port}/");

    using var listener = new HttpListener();
    listener.Prefixes.Add(callbackUri.ToString());
    listener.Start();

    var authUrl = new Uri(string.Format(CultureInfo.InvariantCulture, UnformatedAuthUrl, _apiKey, Uri.EscapeDataString(callbackUri.ToString())));

    try
    {
      Process.Start(new ProcessStartInfo
      {
        FileName = authUrl.ToString(),
        UseShellExecute = true
      });
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Could not open browser. Open this URL manually: " + authUrl, ex);
    }

    using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

    Task<HttpListenerContext> getContextTask = listener.GetContextAsync();
    Task completedTask = await Task.WhenAny(getContextTask, Task.Delay(Timeout.InfiniteTimeSpan, linkedCts.Token));

    if (completedTask != getContextTask)
    {
      listener.Stop();
      throw new TimeoutException("Authentication timed out or was cancelled.");
    }

    var context = getContextTask.Result;

    // Extract token
    var query = context.Request.Url?.Query ?? "";
    var queryParams = HttpUtility.ParseQueryString(query);
    var token = queryParams["token"];

    // Respond to browser
    var response = context.Response;
    var html = "<html><body><h1>Authentication successful. You may close this window.</h1></body></html>";
    var buffer = Encoding.UTF8.GetBytes(html);
    response.ContentLength64 = buffer.Length;
    await response.OutputStream.WriteAsync(buffer, cancellationToken);
    response.OutputStream.Close();
    listener.Stop();

    if (string.IsNullOrWhiteSpace(token))
      throw new InvalidOperationException("No token received from Last.fm callback.");

    return await GetSessionAsync(token, "", "");
  }

  internal static int GetFreePort()
  {
    var listener = new TcpListener(IPAddress.Loopback, 0);
    listener.Start();
    var port = ((IPEndPoint)listener.LocalEndpoint).Port;
    listener.Stop();
    return port;
  }
}