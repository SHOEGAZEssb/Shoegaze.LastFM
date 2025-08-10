using Shoegaze.LastFM.Authentication;
using System.Net;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

      if (!VerifyParameters(parameters, out string? errorMessage))
        return ApiResult<JsonDocument>.Failure(status: null, httpStatus: null, error: errorMessage);

      HttpResponseMessage? response = null;
      try
      {
        if (requireAuth)
        {
          if (string.IsNullOrWhiteSpace(SessionKey))
            return ApiResult<JsonDocument>.Failure(null, null, "Session key is required.");

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

        var jsonDoc = JsonDocument.Parse(json);
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
    /// <param name="errorMessage">Error message in case the given <paramref name="parameters"/> is not valid.</param>
    /// <returns>True if <paramref name="parameters"/> is valid, false otherwise.</returns>
    private static bool VerifyParameters(IDictionary<string, string> parameters, out string? errorMessage)
    {
      errorMessage = null;

      int i = 0;
      foreach (var (key, value) in parameters)
      {
        if (string.IsNullOrEmpty(key))
        {
          errorMessage = $"Parameter key is null/empty at index {i}";
          return false;
        }
        
        if (string.IsNullOrEmpty(value))
        {
          errorMessage = $"Parameter value is null/empty: {key}";
          return false;
        }

        i++;
      }

      return true;
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