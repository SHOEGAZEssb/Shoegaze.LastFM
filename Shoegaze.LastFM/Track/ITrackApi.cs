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

    Task<ApiResult<PagedResult<TrackInfo>>> SearchAsync(
      string track,
      string? artist = null,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    Task<ApiResult> AddTagsAsync(
      string trackName,
      string artistName,
      string tag,
      CancellationToken ct = default);

    Task<ApiResult> AddTagsAsync(
      string trackName,
      string artistName,
      IEnumerable<string> tags,
      CancellationToken ct = default);

    Task<ApiResult> RemoveTagsAsync(
      string trackName,
      string artistName,
      string tag,
      CancellationToken ct = default);

    Task<ApiResult> RemoveTagsAsync(
      string trackName,
      string artistName,
      IEnumerable<string> tags,
      CancellationToken ct = default);

    Task<ApiResult> SetLoveState(
      string trackName,
      string artistName,
      bool loveState,
      CancellationToken ct = default);

    Task<ApiResult<ScrobbleInfo>> UpdateNowPlayingAsync(
      string trackName,
      string artistName,
      string? albumName = null,
      string? albumArtistName = null,
      CancellationToken ct = default);
  }
}
