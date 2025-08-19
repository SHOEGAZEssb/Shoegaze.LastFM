using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Tag
{
  /// <summary>
  /// Access to tag-related api endpoints.
  /// </summary>
  public interface ITagApi
  {
    /// <summary>
    /// Get the metadata for a tag.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the tag metadata, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getInfo"/>.
    Task<ApiResult<TagInfo>> GetInfoAsync(
      string tagName,
      CancellationToken ct = default);

    /// <summary>
    /// Search for similar tags.
    /// Returns tags ranked by similarity, based on listening data.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of similar tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getSimilar"/>.
    [Obsolete("Currently broken on last.fms side, only returns an empty list.")]
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetSimilarAsync(
      string tagName,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top albums tagged by the given tag, ordered by tag count.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of albums, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getTopAlbums"/>.
    Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsAsync(
      string tagName,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top artists tagged by the given tag, ordered by tag count.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of artists, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getTopArtists"/>.
    Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(
      string tagName,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top global tags, sorted by popularity (number of times used).
    /// </summary>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getTopTags"/>.
    Task<ApiResult<PagedResult<TagInfo>>> GetTopTagsAsync(
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top tracks tagged by the given tag, ordered by tag count.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of tracks, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getTopTracks"/>.
    Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(
      string tagName,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get a list of available charts for a tag.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the list of charts, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getWeeklyChartList"/>.
    Task<ApiResult<IReadOnlyList<WeeklyChartInfo>>> GetWeeklyChartListAsync(
      string tagName,
      CancellationToken ct = default);
  }
}