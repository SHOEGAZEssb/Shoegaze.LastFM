using Shoegaze.LastFM.Tag;

namespace Shoegaze.LastFM.Track
{
  public interface ITrackApi
  {
    Task<ApiResult<TrackInfo>> GetInfoByNameAsync(
      string track,
      string artist,
      string? username = null,
      bool autocorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<TrackInfo>> GetInfoByMbidAsync(
      string mbid,
      string? username = null,
      CancellationToken ct = default);

    Task<ApiResult<TrackInfo>> GetCorrectionAsync(
      string track,
      string artist,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarByNameAsync(
      string track,
      string artist,
      bool autocorrect = true,
      int? limit = null,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarByMbidAsync(
      string mbid,
      bool autocorrect = true,
      int? limit = null,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TagInfo>>> GetUserTagsByName(
      string track,
      string artist,
      string? username = null,
      bool autocorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TagInfo>>> GetUserTagsByMbid(
      string mbid,
      string? username = null,
      bool autocorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByName(
      string track,
      string artist,
      bool autocorrect = true,
      CancellationToken ct = default);
  }
}
