using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.User;

/// <summary>
/// Defines different time-periods to get data for.
/// </summary>
public enum TimePeriod
{
  /// <summary>
  /// All-time.
  /// </summary>
  Overall,

  /// <summary>
  /// Last week.
  /// </summary>
  SevenDay,

  /// <summary>
  /// Last month.
  /// </summary>
  OneMonth,

  /// <summary>
  /// Last 3 months.
  /// </summary>
  ThreeMonth,

  /// <summary>
  /// Last 6 months.
  /// </summary>
  SixMonth,

  /// <summary>
  /// Last year.
  /// </summary>
  TwelveMonth
}

/// <summary>
/// Access to user-related Last.fm API methods.
/// </summary>
public interface IUserApi
{
  /// <summary>
  /// Get information about a user profile.
  /// </summary>
  /// <param name="username">Username to look up. If null, uses the authenticated session.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>
  /// Result that contains the user info, or error information.
  /// </returns>
  /// <seealso href="https://www.last.fm/api/show/user.getInfo"/>.
  Task<ApiResult<UserInfo>> GetInfoAsync(string? username = null, CancellationToken ct = default);

  /// <summary>
  /// Get a list of a user's Last.fm friends.
  /// </summary>
  /// <param name="username">Optional username. If null, uses the authenticated session.</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="includeRecentTracks">Whether to include recent track data per user.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>
  /// Result that contains a list with the users friends, or error information.
  /// </returns>
  /// <seealso href="https://www.last.fm/api/show/user.getFriends"/>.
  Task<ApiResult<PagedResult<UserInfo>>> GetFriendsAsync(
      string username,
      bool includeRecentTracks = false,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

  /// <summary>
  /// Get the loved tracks of a user.
  /// </summary>
  /// <param name="username">User to get loved tracks for.</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of the loved tracks, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getLovedTracks"/>.
  Task<ApiResult<PagedResult<TrackInfo>>> GetLovedTracksAsync(
    string username,
    int? limit = null,
    int? page = null,
    CancellationToken ct = default);

  /// <summary>
  /// Get the top tracks listened to by a user.
  /// </summary>
  /// <param name="username">User to get top tracks for.</param>
  /// <param name="period">Time period (defaults to <see cref="TimePeriod.Overall"/>).</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of the top tracks, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getTopTracks"/>.
  Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(
    string username,
    TimePeriod? period = null,
    int? limit = null,
    int? page = null,
    CancellationToken ct = default);

  /// <summary>
  /// Get a users recently played tracks.
  /// </summary>
  /// <param name="username">User to get recent tracks for.</param>
  /// <param name="extended">Wether to include extended data in each artist,
  /// and whether or not the user has loved each track.</param>
  /// <param name="fromDate">Beginning timestamp of a range - only fetch scrobbles after this time.</param>
  /// <param name="toDate"> End timestamp of a range - only fetch scrobbles before this time.</param>
  /// <param name="ignoreNowPlaying">Wether the currently "now playing" track should be filtered out in the result.</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of the recent tracks, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getRecentTracks"/>.
  Task<ApiResult<PagedResult<TrackInfo>>> GetRecentTracksAsync(
    string username,
    bool? extended = null,
    DateTimeOffset? fromDate = null,
    DateTimeOffset? toDate = null,
    bool ignoreNowPlaying = false,
    int? limit = null,
    int? page = null,
    CancellationToken ct = default);

  /// <summary>
  /// Get the top tags used by a user.
  /// </summary>
  /// <param name="username">User to get top tags for.</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of top tags, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getTopTags"/>.
  Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsAsync(
    string username,
    int? limit = null,
    CancellationToken ct = default);

  /// <summary>
  /// Get the users taggings.
  /// </summary>
  /// <typeparam name="T">The type of object to get the users taggings for.
  /// Supports <see cref="ArtistInfo"/>, <see cref="TrackInfo"/> and <see cref="AlbumInfo"/>.</typeparam>
  /// <param name="username">User whose taggings to get.</param>
  /// <param name="tag">Tag to get taggings for.</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of taggings, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getPersonalTags"/>.
  Task<ApiResult<IReadOnlyList<T>>> GetPersonalTagsAsync<T>(
    string username,
    string tag,
    int? limit = null,
    int? page = null,
    CancellationToken ct = default) where T : IUserTagable;

  /// <summary>
  /// Get the top artists listened to by a user.
  /// </summary>
  /// <param name="username">User to get top artists for.</param>
  /// <param name="period">Time period (defaults to <see cref="TimePeriod.Overall"/>).</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of top artists, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getTopArtists"/>.
  Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(
    string username,
    TimePeriod? period = null,
    int? limit = null,
    int? page = null,
    CancellationToken ct = default);

  /// <summary>
  /// Get the top albums listened to by a user.
  /// </summary>
  /// <param name="username">User to get top albums for.</param>
  /// <param name="period">Time period (defaults to <see cref="TimePeriod.Overall"/>).</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of top albums, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getTopAlbums"/>.
  Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsAsync(
    string username,
    TimePeriod? period = null,
    int? limit = null,
    int? page = null,
    CancellationToken ct = default);

  /// <summary>
  /// Get a list of available charts for a user.
  /// </summary>
  /// <param name="username">User to get the chart list for.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains the list of charts, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getWeeklyChartList"/>.
  Task<ApiResult<IReadOnlyList<WeeklyChartInfo>>> GetWeeklyChartListAsync(
    string username,
    CancellationToken ct = default);

  /// <summary>
  /// Get a weekly chart for a user.
  /// </summary>
  /// <typeparam name="T">The type of object to get the weekly chart for.
  /// Supports <see cref="ArtistInfo"/>, <see cref="TrackInfo"/> and <see cref="AlbumInfo"/>.</typeparam>
  /// <param name="username">User to get weekly chart for.</param>
  /// <param name="fromDate">The date at which the chart should start from.</param>
  /// <param name="toDate">The date at which the chart should end on</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of objects for the chart, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getWeeklyArtistChart"/>.
  /// <seealso href="https://www.last.fm/api/show/user.getWeeklyTrackChart"/>.
  /// <seealso href="https://www.last.fm/api/show/user.getWeeklyAlbumChart"/>.
  Task<ApiResult<IReadOnlyList<T>>> GetWeeklyChartAsync<T>(
    string username,
    DateTimeOffset? fromDate = null,
    DateTimeOffset? toDate = null,
    CancellationToken ct = default) where T : IUserChartable;
}