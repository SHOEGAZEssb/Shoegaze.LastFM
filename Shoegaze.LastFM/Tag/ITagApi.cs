using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;

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
  }
}