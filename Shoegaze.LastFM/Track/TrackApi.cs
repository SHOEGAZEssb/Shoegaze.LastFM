namespace Shoegaze.LastFM.Track
{
  public class TrackApi : ITrackApi
  {
    private readonly ILastfmRequestInvoker _invoker;

    internal TrackApi(ILastfmRequestInvoker invoker) => _invoker = invoker;

    public async Task<ApiResult<TrackInfo>> GetInfoByNameAsync(
      string track,
      string artist,
      string? username = null,
      bool autocorrect = false,
      CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["track"] = track,
        ["artist"] = artist,
        ["autocorrect"] = autocorrect ? "1" : "0"
      };

      if (!string.IsNullOrWhiteSpace(username))
        parameters["username"] = username;

      var result = await _invoker.SendAsync("track.getInfo", parameters, false, ct);

      if (!result.IsSuccess || result.Data == null)
        return ApiResult<TrackInfo>.Failure(result.Status, result.HttpStatusCode, result.ErrorMessage);

      try
      {
        var trackInfo = TrackInfo.FromJson(result.Data.RootElement.GetProperty("track"));
        if (username == null)
          trackInfo.UserPlayCount = null; // manual fix for duplicate playcount property possibility (if username is null, userplaycount will not be available)
        return ApiResult<TrackInfo>.Success(trackInfo, result.HttpStatusCode);
      }
      catch (Exception ex)
      {
        return ApiResult<TrackInfo>.Failure(ApiStatusCode.UnknownError, result.HttpStatusCode, "Failed to parse track info: " + ex.Message);
      }
    }

    public async Task<ApiResult<TrackInfo>> GetInfoByMbidAsync(
      string mbid,
      string? username = null,
      CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["mbid"] = mbid
      };

      if (!string.IsNullOrWhiteSpace(username))
        parameters["username"] = username;

      var result = await _invoker.SendAsync("track.getInfo", parameters, false, ct);

      if (!result.IsSuccess || result.Data == null)
        return ApiResult<TrackInfo>.Failure(result.Status, result.HttpStatusCode, result.ErrorMessage);

      try
      {
        var trackInfo = TrackInfo.FromJson(result.Data.RootElement.GetProperty("track"));
        if (username == null)
          trackInfo.UserPlayCount = null; // manual fix for duplicate playcount property possibility (if username is null, userplaycount will not be available)
        return ApiResult<TrackInfo>.Success(trackInfo, result.HttpStatusCode);
      }
      catch (Exception ex)
      {
        return ApiResult<TrackInfo>.Failure(ApiStatusCode.UnknownError, result.HttpStatusCode, "Failed to parse track info: " + ex.Message);
      }
    }
  }

}
