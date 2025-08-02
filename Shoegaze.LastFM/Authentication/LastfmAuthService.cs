using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Shoegaze.LastFM.Authentication;

/// <summary>
/// Handles OAuth 1.0a authentication with the Last.fm API.
/// </summary>
public class LastfmAuthService(HttpClient httpClient, string apiKey, string apiSecret, string callbackUrl) : IAuthService
{
  private const string RequestTokenUrl = "https://www.last.fm/api/auth/?api_key={0}&cb={1}";
  private const string SessionUrl = "https://ws.audioscrobbler.com/2.0/";

  private readonly string _apiKey = apiKey;
  private readonly string _apiSecret = apiSecret;
  private readonly string _callbackUrl = callbackUrl;
  private readonly HttpClient _http = httpClient;

  /// <inheritdoc />
  public Task<Uri> GetAuthorizationUrlAsync()
  {
    var authUrl = string.Format(RequestTokenUrl, _apiKey, HttpUtility.UrlEncode(_callbackUrl));
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
    using var response = await _http.PostAsync(SessionUrl, content);
    response.EnsureSuccessStatusCode();

    var json = await response.Content.ReadAsStringAsync();
    using var doc = JsonDocument.Parse(json);

    var session = doc.RootElement.GetProperty("session");
    return new AuthSession
    {
      Username = session.GetProperty("name").GetString()!,
      SessionKey = session.GetProperty("key").GetString()!
    };
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

    var authUrl = new Uri($"https://www.last.fm/api/auth/?api_key={_apiKey}&cb={Uri.EscapeDataString(callbackUri.ToString())}");

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