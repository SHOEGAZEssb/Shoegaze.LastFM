using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Geo
{
  /// <summary>
  /// Access to geo-related api endpoints.
  /// </summary>
  public interface IGeoApi
  {
    /// <summary>
    /// Get the global top artists by country.
    /// </summary>
    /// <param name="country">A country name, as defined by the ISO 3166-1 country names standard.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top artists, or error information.</returns>
    public Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(
      string country,
      int? limit = null,
      int? page = null,
      CancellationToken ct  = default);

    /// <summary>
    /// Get the global top tracks by country.
    /// </summary>
    /// <param name="country">A country name, as defined by the ISO 3166-1 country names standard.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top tracks, or error information.</returns>
    public Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(
      string country,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);
  }
}