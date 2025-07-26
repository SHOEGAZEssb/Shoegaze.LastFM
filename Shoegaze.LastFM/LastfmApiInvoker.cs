using Shoegaze.LastFM.Authentication;
using System.Text.Json;

namespace Shoegaze.LastFM
{
  internal class LastfmApiInvoker : ILastfmRequestInvoker
  {
    public string ApiKey { get; }
    public string ApiSecret { get; }
    public string? SessionKey { get; set; }

    private readonly HttpClient _http;

    public LastfmApiInvoker(string apiKey, string apiSecret, HttpClient httpClient)
    {
      ApiKey = apiKey;
      ApiSecret = apiSecret;
      _http = httpClient;
    }

    public async Task<ApiResult<JsonDocument>> SendAsync(string method, IDictionary<string, string> parameters, bool requireAuth = false, CancellationToken ct = default)
    {
      parameters["method"] = method;
      parameters["api_key"] = ApiKey;
      parameters["format"] = "json";

      if (requireAuth)
      {
        if (string.IsNullOrWhiteSpace(SessionKey))
          return ApiResult<JsonDocument>.Failure(ApiStatusCode.AuthenticationRequired, 401, "Session key is required.");

        parameters["sk"] = SessionKey;

        var apiSig = LastfmAuthService.GenerateApiSignature(parameters, ApiSecret);
        parameters["api_sig"] = apiSig;
      }

      try
      {
        using var content = new FormUrlEncodedContent(parameters);
        using var response = await _http.PostAsync("https://ws.audioscrobbler.com/2.0/", content, ct);
        var json = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
          return ApiResult<JsonDocument>.Failure(
              status: ApiStatusCode.HttpError,
              httpStatus: (int)response.StatusCode,
              error: $"HTTP {response.StatusCode}: {json}"
          );
        }

        return ApiResult<JsonDocument>.Success(JsonDocument.Parse(json), (int)response.StatusCode);
      }
      catch (HttpRequestException ex)
      {
        return ApiResult<JsonDocument>.Failure(ApiStatusCode.NetworkError, 0, ex.Message);
      }
      catch (Exception ex)
      {
        return ApiResult<JsonDocument>.Failure(ApiStatusCode.UnknownError, 0, ex.Message);
      }
    }
  }
}