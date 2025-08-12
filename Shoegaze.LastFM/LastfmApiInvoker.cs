using Shoegaze.LastFM.Authentication;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Shoegaze.LastFM
{
  internal class LastfmApiInvoker(string apiKey, string apiSecret, HttpClient httpClient) : ILastfmApiInvoker
  {
    public string ApiKey { get; } = apiKey;
    public string ApiSecret { get; } = apiSecret;
    public string? SessionKey { get; set; }

    private readonly HttpClient _http = httpClient;

    public async Task<ApiResult<JsonDocument>> SendAsync(string method, IDictionary<string, string> parameters, bool requireAuth = false, CancellationToken ct = default)
    {
      parameters["method"] = method;
      parameters["api_key"] = ApiKey;
      parameters["format"] = "json";

      ThrowOnInvalidParameter(parameters);

      HttpResponseMessage? response = null;
      try
      {
        if (requireAuth)
        {
          if (string.IsNullOrWhiteSpace(SessionKey))
            throw new Exception("Client is not authenticated. Authentication is required for this api call.");

          parameters["sk"] = SessionKey;

          var apiSig = LastfmAuthService.GenerateApiSignature(parameters, ApiSecret);
          parameters["api_sig"] = apiSig;

          // Send as POST with ResponseHeadersRead to avoid buffering full content
          using var content = new FormUrlEncodedContent(parameters);
          using var request = new HttpRequestMessage(HttpMethod.Post, "https://ws.audioscrobbler.com/2.0/")
          {
            Content = content
          };
          response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        }
        else
        {
          // Send as GET with ResponseHeadersRead to avoid buffering full content
          var queryBuilder = new StringBuilder();
          bool first = true;
          foreach (var kv in parameters)
          {
            if (!first)
              queryBuilder.Append('&');
            first = false;
            queryBuilder
              .Append(Uri.EscapeDataString(kv.Key))
              .Append('=')
              .Append(Uri.EscapeDataString(kv.Value));
          }
          var url = $"https://ws.audioscrobbler.com/2.0/?{queryBuilder}";
          response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        }

        await using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        var jsonDoc = await JsonDocument.ParseAsync(stream, cancellationToken: ct).ConfigureAwait(false);
        if (TryParseLastFmError(jsonDoc.RootElement, out var lfmCode, out var msg))
          return ApiResult<JsonDocument>.Failure(lfmCode, response.StatusCode, msg);
        else
          return ApiResult<JsonDocument>.Success(jsonDoc, response.StatusCode);
      }
      catch (HttpRequestException ex)
      {
        return ApiResult<JsonDocument>.Failure(null, ex.StatusCode, ex.Message);
      }
      catch (Exception ex)
      {
        return ApiResult<JsonDocument>.Failure(null, response?.StatusCode, ex.Message);
      }
      finally
      {
        response?.Dispose();
      }
    }

    /// <summary>
    /// Check each parameter in the given <paramref name="parameters"/> for validity (not empty, not null).
    /// </summary>
    /// <param name="parameters">Parameters to check.</param>
    private static void ThrowOnInvalidParameter(IDictionary<string, string> parameters)
    {
      int i = 0;
      foreach (var (key, value) in parameters)
      {
        if (string.IsNullOrEmpty(key))
          throw new ArgumentNullException(nameof(parameters), $"Parameter key is null/empty at index {i}");
        
        if (string.IsNullOrEmpty(value))
          throw new ArgumentNullException(nameof(parameters), $"Parameter value is null/empty: {key}");

        i++;
      }
    }

    private static bool TryParseLastFmError(JsonElement root, out LastFmStatusCode statusCode, out string? errorMessage)
    {
      statusCode = LastFmStatusCode.UnknownError;
      errorMessage = null;
      try
      {
        if (root.TryGetProperty("error", out var errorProp) && JsonHelper.TryParseNumber(errorProp, out int error))
        {
          statusCode = ParseEnumOrDefault(error, LastFmStatusCode.UnknownError);
          if (root.TryGetProperty("message", out var messageProp))
            errorMessage = messageProp.GetString();

          return true;
        }

        return false;
      }
      catch
      {
        return false;
      }
    }

    private static TEnum ParseEnumOrDefault<TEnum>(int value, TEnum defaultValue) where TEnum : struct, Enum
    {
      return Enum.IsDefined(typeof(TEnum), value)
        ? (TEnum)(object)value
        : defaultValue;
    }
  }
}