namespace Shoegaze.LastFM.Track
{
  public interface ITrackApi
  {
    Task<ApiResult<TrackInfo>> GetInfoByNameAsync(
      string track,
      string artist,
      string? username = null,
      bool autocorrect = false,
      CancellationToken ct = default);

    Task<ApiResult<TrackInfo>> GetInfoByMbidAsync(
      string mbid,
      string? username = null,
      CancellationToken ct = default);

    Task<ApiResult<TrackInfo>> GetCorrectionAsync(
      string track,
      string artist,
      CancellationToken ct = default);
  }
}
