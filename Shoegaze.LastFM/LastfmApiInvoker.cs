using Shoegaze.LastFM.Authentication;
using System.Net;
using System.Text.Json;

namespace Shoegaze.LastFM
{
  internal class LastfmApiInvoker(string apiKey, string apiSecret, HttpClient httpClient) : ILastfmRequestInvoker
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

      HttpResponseMessage? response = null;
      try
      {
        if (requireAuth)
        {
          if (string.IsNullOrWhiteSpace(SessionKey))
            return ApiResult<JsonDocument>.Failure(LastFmStatusCode.AuthenticationFailed, HttpStatusCode.Unauthorized, "Session key is required.");

          parameters["sk"] = SessionKey;

          var apiSig = LastfmAuthService.GenerateApiSignature(parameters, ApiSecret);
          parameters["api_sig"] = apiSig;

          // send as post
          using var content = new FormUrlEncodedContent(parameters);
          response = await _http.PostAsync("https://ws.audioscrobbler.com/2.0/", content, ct);
        }
        else
        {
          // Send as GET
          var query = string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
          response = await _http.GetAsync($"https://ws.audioscrobbler.com/2.0/?{query}", ct);
        }

        var json = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
          return ApiResult<JsonDocument>.Failure(
              status: LastFmStatusCode.UnknownError,
              httpStatus: response.StatusCode,
              error: $"HTTP {response.StatusCode}: {json}"
          );
        }

        var jsonDoc = JsonDocument.Parse(json);
        if (TryParseLastFmError(jsonDoc.RootElement, out var lfmCode, out var msg))
          return ApiResult<JsonDocument>.Failure(lfmCode, response.StatusCode, msg);
        else
          return ApiResult<JsonDocument>.Success(jsonDoc, response.StatusCode);
      }
      catch (HttpRequestException ex)
      {
        return ApiResult<JsonDocument>.Failure(LastFmStatusCode.UnknownError, ex.StatusCode, ex.Message);
      }
      catch (Exception ex)
      {
        return ApiResult<JsonDocument>.Failure(LastFmStatusCode.UnknownError, 0, ex.Message);
      }
      finally
      {
        response?.Dispose();
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