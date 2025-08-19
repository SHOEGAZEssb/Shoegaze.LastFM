using Shoegaze.LastFM.Artist;

namespace Shoegaze.LastFM.Library
{
  /// <summary>
  /// Access to library-related api endpoints.
  /// </summary>
  public interface ILibraryApi
  {
    /// <summary>
    /// Get a list of all the artists in a users library.
    /// </summary>
    /// <param name="username">Name of the user.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of artists in the users library, or error information.</returns>
    public Task<ApiResult<PagedResult<ArtistInfo>>> GetArtistsAsync(
      string username,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);
  }
}