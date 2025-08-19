using Shoegaze.LastFM.Tag;

namespace Shoegaze.LastFM.Album
{
  /// <summary>
  /// Interface for an object that gives access to album-related api endpoints.
  /// </summary>
  public interface IAlbumApi
  {
    /// <summary>
    /// Get the metadata for an album.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="username">The username for the context of the request.
    /// If supplied, the users playcount for this album is included
    /// in the response.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names,
    /// returning the correct version instead</param>
    /// <param name="language">The language to return the biography in, expressed as an ISO 639 alpha-2 code.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result object that contains the album metadata, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.getInfo"/>.
    Task<ApiResult<AlbumInfo>> GetInfoByNameAsync(
      string albumName,
      string artistName,
      string? username = null,
      bool autoCorrect = true,
      string? language = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the metadata for an album.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the album.</param>
    /// <param name="username">The username for the context of the request.
    /// If supplied, the users playcount for this album is included
    /// in the response.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names,
    /// returning the correct version instead.</param>
    /// <param name="language">The language to return the biography in, expressed as an ISO 639 alpha-2 code.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response from the last.fm api that
    /// contains the album metadata, or error information.</returns>
    /// <remarks>
    /// <see href="https://www.last.fm/api/show/album.getInfo"/>.
    /// </remarks>
    Task<ApiResult<AlbumInfo>> GetInfoByMbidAsync(
      string mbid,
      string? username = null,
      bool autoCorrect = true,
      string? language = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the tags applied by an individual user to an album.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">name of the artist.</param>
    /// <param name="username">User to look up. If null, the authenticated session username will be used.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names,
    /// returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result object that contains the tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.getTags"/>.
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByNameAsync(
      string albumName,
      string artistName,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    /// <summary>
    /// Get the tags applied by an individual user to an album.
    /// </summary>
    /// <param name="mbid">Musibrainz ID of the album.</param>
    /// <param name="username">User to look up. If null, the authenticated session username will be used.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names,
    /// returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result object that contains the tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.getTags"/>.
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByMbidAsync(
      string mbid,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top tags for an album, ordered by popularity.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names,
    /// returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result object that contains the tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.getTopTags"/>.
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByNameAsync(
      string albumName,
      string artistName,
      bool autoCorrect = true,
      CancellationToken ct = default);

    // currently broken?
    //Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByMbidAsync(
    //  string mbid,
    //  bool autoCorrect = true,
    //  CancellationToken ct = default);

    /// <summary>
    /// Search for an album by name.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="limit">Number of results to fetch per page. Defaults to 30.</param>
    /// <param name="page">The page number to fetch. Defaults to first page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the found albums sorted by relevancy, or error information..</returns>
    /// <seealso href="https://www.last.fm/api/show/album.search"/>.
    Task<ApiResult<PagedResult<AlbumInfo>>> SearchAsync(
      string albumName,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Tag an album using an user supplied tag.
    /// This method requires authentication.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tag">Tag to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.addTags"/>.
    Task<ApiResult> AddTagsAsync(
      string albumName,
      string artistName,
      string tag,
      CancellationToken ct = default);

    /// <summary>
    /// Tag an album using a list of user supplied tags.
    /// This method requires authentication.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tags">Tags to add. Maximum of 10 allowed.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.addTags"/>.
    Task<ApiResult> AddTagsAsync(
      string albumName,
      string artistName,
      IEnumerable<string> tags,
      CancellationToken ct = default);

    /// <summary>
    /// Remove a users tag from an album.
    /// This method requires authentication.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tag">Tag to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.removeTag"/>.
    Task<ApiResult> RemoveTagsAsync(
      string albumName,
      string artistName,
      string tag,
      CancellationToken ct = default);

    /// <summary>
    /// Remove a users list of tags from an album.
    /// This method requires authentication.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tags">List of tags to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.removeTag"/>.
    Task<ApiResult> RemoveTagsAsync(
      string albumName,
      string artistName,
      IEnumerable<string> tags,
      CancellationToken ct = default);
  }
}