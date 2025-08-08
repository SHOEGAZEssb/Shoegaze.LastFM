using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Tag
{
  public interface ITagApi
  {
    Task<ApiResult<TagInfo>> GetInfoAsync(
      string tagName,
      CancellationToken ct = default);

    /// <summary>
    /// Get tags similar to the given <paramref name="tagName"/>.
    /// </summary>
    /// <param name="tagName">Tag to get similar tags for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of similar tags.</returns>
    /// <remarks>
    /// Broken on last.fms side currently.
    /// Will only return an empty list.
    /// </remarks>
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetSimilarAsync(
      string tagName,
      CancellationToken ct = default);

    Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsAsync(
      string tagName,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(
      string tagName,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Fetches the global top tags on Last.fm, sorted by number of times used.
    /// </summary>
    /// <param name="limit">Maximum items per page (maximum is 1000).</param>
    /// <param name="page">The page to get.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task.</returns>
    Task<ApiResult<PagedResult<TagInfo>>> GetTopTagsAsync(
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(
      string tagName,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<WeeklyChartInfo>>> GetWeeklyChartListAsync(
      string tagName,
      CancellationToken ct = default);
  }
}