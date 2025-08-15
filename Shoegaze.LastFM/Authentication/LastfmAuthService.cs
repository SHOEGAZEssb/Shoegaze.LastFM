using System.Diagnostics;
using System.Net;
using System.Net.Http;
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

  /// <inheritdoc />
  public Task<Uri> GetAuthorizationUrlAsync()
  {
    var encodedCallback = HttpUtility.UrlEncode(_callbackUrl);
    var authUrl = string.Format(CultureInfo.InvariantCulture, UnformatedAuthUrl, _apiKey, encodedCallback);
    return Task.FromResult(new Uri(authUrl));
  }

  /// <inheritdoc />
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