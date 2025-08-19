using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Chart
{
  /// <summary>
  /// Access to chart-related api endpoints.
  /// </summary>
  public interface IChartApi
  {
    /// <summary>
    /// Get the global top artists.
    /// </summary>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of the global top artists, or error information.</returns>
    Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the global top tags.
    /// </summary>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of the global top tags, or error information.</returns>
    Task<ApiResult<PagedResult<TagInfo>>> GetTopTagsAsync(
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the global top tracks.
    /// </summary>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of the global top tracks, or error information.</returns>
    Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);
  }
}