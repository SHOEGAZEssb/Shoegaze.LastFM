using Shoegaze.LastFM.Tag;

namespace Shoegaze.LastFM.Track
{
  /// <summary>
  /// Access to track-related api endpoints.
  /// </summary>
  public interface ITrackApi
  {
    /// <summary>
    /// Get the metadata for a track.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist.</param>
    /// <param name="username">The username for the context of the request.
    /// If supplied, the user's playcount for this track and whether they have loved the track is included in the response.</param>
    /// <param name="autoCorrect">Transform misspelled artist and track names into correct artist and track names, returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the track metadata, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getInfo"/>.
    Task<ApiResult<TrackInfo>> GetInfoByNameAsync(
      string track,
      string artist,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    /// <summary>
    /// Get the metadata for a track.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the track.</param>
    /// <param name="username">The username for the context of the request.
    /// If supplied, the user's playcount for this track and whether they have loved the track is included in the response.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the track metadata, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getInfo"/>.
    Task<ApiResult<TrackInfo>> GetInfoByMbidAsync(
      string mbid,
      string? username = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the corrected name for a track.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the corrected track, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getCorrection"/>.
    Task<ApiResult<TrackInfo>> GetCorrectionAsync(
      string track,
      string artist,
      CancellationToken ct = default);

    /// <summary>
    /// Get similar tracks to the given track.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist.</param>
    /// <param name="autoCorrect">Transform misspelled artist and track names into correct artist and track names, returning the correct version instead.</param>
    /// <param name="limit">Maximum number of similar tracks to return.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of similar tracks, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getSimilar"/>.
    Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarByNameAsync(
      string track,
      string artist,
      bool autoCorrect = true,
      int? limit = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get similar tracks to the given track.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the track.</param>
    /// <param name="limit">Maximum number of similar tracks to return.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of similar tracks, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getSimilar"/>.
    Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarByMbidAsync(
      string mbid,
      int? limit = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the tags applied by an individual user to a track.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist.</param>
    /// <param name="username">Username to look up. If null, uses the authenticated session.</param>
    /// <param name="autoCorrect">Transform misspelled artist and track names into correct artist and track names, returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getTags"/>.
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByNameAsync(
      string track,
      string artist,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    /// <summary>
    /// Get the tags applied by an individual user to a track.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the track.</param>
    /// <param name="username">Username to look up. If null, uses the authenticated session.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getTags"/>.
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByMbidAsync(
      string mbid,
      string? username = null,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top tags for a track.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist.</param>
    /// <param name="autoCorrect">Transform misspelled artist and track names into correct artist and track names, returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getTopTags"/>.
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByNameAsync(
      string track,
      string artist,
      bool autoCorrect = true,
      CancellationToken ct = default);

    /// <summary>
    /// Get the top tags for a track.
    /// </summary>
    /// <param name="mbid">MusicBrainz ID of the track.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getTopTags"/>.
    [Obsolete("Currently broken on last.fms side, only returns an empty list.")]
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByMbidAsync(
      string mbid,
      CancellationToken ct = default);

    /// <summary>
    /// Search for a track by name.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist to narrow your search.</param>
    /// <param name="limit">Number of results per page (defaults to 30).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of found track sorted by relevance, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.search"/>.
    Task<ApiResult<PagedResult<TrackInfo>>> SearchAsync(
      string track,
      string? artist = null,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    /// <summary>
    /// Tag an album using a user supplied tag.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tag">Tag to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.addTags"/>.
    Task<ApiResult> AddTagsAsync(
      string trackName,
      string artistName,
      string tag,
      CancellationToken ct = default);

    /// <summary>
    /// Tag an album using a list of user supplied tag.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tags">Tags to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.addTags"/>.
    Task<ApiResult> AddTagsAsync(
      string trackName,
      string artistName,
      IEnumerable<string> tags,
      CancellationToken ct = default);

    /// <summary>
    /// Remove a users tag from an artist.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tag">Tag to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.removeTag"/>.
    Task<ApiResult> RemoveTagsAsync(
      string trackName,
      string artistName,
      string tag,
      CancellationToken ct = default);

    /// <summary>
    /// Remove a list of users tag from an artist.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tags">Tags to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.removeTag"/>.
    Task<ApiResult> RemoveTagsAsync(
      string trackName,
      string artistName,
      IEnumerable<string> tags,
      CancellationToken ct = default);

    /// <summary>
    /// Loves or unloves a track.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="loveState">The love state to set.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.love"/>.
    /// <seealso href="https://www.last.fm/api/show/track.unlove"/>.
    Task<ApiResult> SetLoveState(
      string trackName,
      string artistName,
      bool loveState,
      CancellationToken ct = default);

    /// <summary>
    /// Notify last.fm that a user has started listening to a track.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="albumArtistName">Name of the album artist.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.updateNowPlaying"/>.
    Task<ApiResult> UpdateNowPlayingAsync(
      string trackName,
      string artistName,
      string? albumName = null,
      string? albumArtistName = null,
      CancellationToken ct = default);

    /// <summary>
    /// Add a single track-play to a users profile.
    /// This method requires authentication.
    /// </summary>
    /// <param name="scrobble">The scrobble data to scrobble.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a scrobble response, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.scrobble"/>.
    Task<ApiResult<ScrobbleInfo>> ScrobbleAsync(
      ScrobbleData scrobble,
      CancellationToken ct = default);

    /// <summary>
    /// Add a batch of track-play to a users profile.
    /// This method requires authentication.
    /// </summary>
    /// <param name="scrobbles">The scrobble data to scrobble (maximum of 50).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a scrobble response, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.scrobble"/>
    Task<ApiResult<IReadOnlyList<ScrobbleInfo>>> ScrobbleAsync(
      IEnumerable<ScrobbleData> scrobbles,
      CancellationToken ct = default);
  }
}