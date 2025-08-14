using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Artist
{
  public interface IArtistApi
  {
    Task<ApiResult<ArtistInfo>> GetInfoByNameAsync(
      string artistName,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<ArtistInfo>> GetInfoByMbidAsync(
      string mbid,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<ArtistInfo>>> GetSimilarByNameAsync(
      string artistName,
      bool autoCorrect = true,
      int? limit = null,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<ArtistInfo>>> GetSimilarByMbidAsync(
      string mbid,
      bool autoCorrect = true,
      int? limit = null,
      CancellationToken ct = default);

    Task<ApiResult<ArtistInfo>> GetCorrectionAsync(
      string artistName,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByNameAsync(
      string artistName,
      string? username = null,
      bool autocorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByMbidAsync(
      string mbid,
      string? username = null,
      bool autocorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsByNameAsync(
      string artistName,
      bool autoCorrect = true,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsByMbidAsync(
      string mbid,
      bool autoCorrect = true,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByNameAsync(
      string artistName,
      bool autoCorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByMbidAsync(
      string mbid,
      bool autoCorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksByNameAsync(
      string artistName,
      bool autoCorrect = true,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksByMbidAsync(
      string mbid,
      bool autoCorrect = true,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    Task<ApiResult<PagedResult<ArtistInfo>>> SearchAsync(
      string artistName,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    Task<ApiResult> AddTagsAsync(
      string artistName,
      string tag,
      CancellationToken ct = default);

    Task<ApiResult> AddTagsAsync(
      string artistName,
      IEnumerable<string> tags,
      CancellationToken ct = default);

    Task<ApiResult> RemoveTagAsync(
      string artistName,
      string tag,
      CancellationToken ct = default);
  }
}
