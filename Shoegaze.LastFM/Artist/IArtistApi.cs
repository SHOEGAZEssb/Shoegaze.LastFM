using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Artist
{
  /// <summary>
  /// Access to artist-related api endpoints.
  /// </summary>
  public interface IArtistApi
  {
    /// <summary>
    /// Get the metadata for an artist.
    /// </summary>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="username">The username for the context of the request.
    /// If supplied, the user's playcount for this artist is included in the response.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names, returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the artist metadata, or error information.</returns>
    /// <remarks>
    /// The <see cref="ArtistInfo.Biography"/> is truncated at 300 characters.
    /// </remarks>
    /// <seealso href="https://www.last.fm/api/show/artist.getInfo"/>.
    Task<ApiResult<ArtistInfo>> GetInfoByNameAsync(
      string artistName,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    /// <summary>
    /// Get the metadata for an artist.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the artist.</param>
    /// <param name="username">The username for the context of the request.
    /// If supplied, the user's playcount for this artist is included in the response.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names, returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the artist metadata, or error information.</returns>
    /// <remarks>
    /// The <see cref="ArtistInfo.Biography"/> is truncated at 300 characters.
    /// </remarks>
    /// <seealso href="https://www.last.fm/api/show/artist.getInfo"/>.
    Task<ApiResult<ArtistInfo>> GetInfoByMbidAsync(
      string mbid,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    /// <summary>
    /// Get similar artists to the given artist.
    /// </summary>
    /// <param name="artistName">The artist name.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names, returning the correct version instead.</param>
    /// <param name="limit">Limit the number of similar artists returned.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of similar artists, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.getSimilar"/>.
    Task<ApiResult<IReadOnlyList<ArtistInfo>>> GetSimilarByNameAsync(
      string artistName,
      bool autoCorrect = true,
      int? limit = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get similar artists to the given artist.
    /// </summary>
    /// <param name="mbid">Musicbrainz id of the artist.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names, returning the correct version instead.</param>
    /// <param name="limit">Limit the number of similar artists returned.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of similar artists, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.getSimilar"/>.
    Task<ApiResult<IReadOnlyList<ArtistInfo>>> GetSimilarByMbidAsync(
      string mbid,
      bool autoCorrect = true,
      int? limit = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the corrected name for an artist.
    /// </summary>
    /// <param name="artistName">The artist name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the corrected artist, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.getCorrection"/>.
    Task<ApiResult<ArtistInfo>> GetCorrectionAsync(
      string artistName,
      CancellationToken ct = default);

    /// <summary>
    /// Get the tags applied by an individual user to an artist.
    /// </summary>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="username">User to get applied tags for. If null, uses the authenticated session.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names, returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of applied tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.getTags"/>.
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByNameAsync(
      string artistName,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    /// <summary>
    /// Get the tags applied by an individual user to an artist.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the artist.</param>
    /// <param name="username">User to get applied tags for. If null, uses the authenticated session.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of applied tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.getTags"/>.
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByMbidAsync(
      string mbid,
      string? username = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top albums for an artist, ordered by popularity.
    /// </summary>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names, returning the correct version instead.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top albums, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.getTopAlbums"/>.
    Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsByNameAsync(
      string artistName,
      bool autoCorrect = true,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top albums for an artist, ordered by popularity.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the artist.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top albums, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.getTopAlbums"/>.
    Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsByMbidAsync(
      string mbid,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top tags for an artist, ordered by popularity.
    /// </summary>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names, returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.getTopTags"/>.
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByNameAsync(
      string artistName,
      bool autoCorrect = true,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top tags for an artist, ordered by popularity.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the artist.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.getTopTags"/>.
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByMbidAsync(
      string mbid,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top track of an artist, ordered by popularity.
    /// </summary>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names, returning the correct version instead.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top tracks, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.getTopTracks"/>.
    Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksByNameAsync(
      string artistName,
      bool autoCorrect = true,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top track of an artist, ordered by popularity.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the artist.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top tracks, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.getTopTracks"/>.
    Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksByMbidAsync(
      string mbid,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Search for an artist by name.
    /// </summary>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="limit">Number of results per page (defaults to 30).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of found artists, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.search"/>.
    Task<ApiResult<PagedResult<ArtistInfo>>> SearchAsync(
      string artistName,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Tag an artist using a user supplied tag.
    /// This method requires authentication.
    /// </summary>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tag">Tag to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.addTags"/>.
    Task<ApiResult> AddTagsAsync(
      string artistName,
      string tag,
      CancellationToken ct = default);

    /// <summary>
    /// Tag an artist using a list of user supplied tags.
    /// This method requires authentication.
    /// </summary>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tags">Tags to add. Maximum of 10 allowed.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.addTags"/>.
    Task<ApiResult> AddTagsAsync(
      string artistName,
      IEnumerable<string> tags,
      CancellationToken ct = default);

    /// <summary>
    /// Remove a users tag from an artist.
    /// This method requires authentication.
    /// </summary>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tag">Tag to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.removeTag"/>.
    Task<ApiResult> RemoveTagsAsync(
      string artistName,
      string tag,
      CancellationToken ct = default);

    /// <summary>
    /// Remove a list of users tags from an artist.
    /// This method requires authentication.
    /// </summary>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tags">Tags to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/artist.removeTag"/>.
    Task<ApiResult> RemoveTagsAsync(
      string artistName,
      IEnumerable<string> tags,
      CancellationToken ct = default);
  }
}