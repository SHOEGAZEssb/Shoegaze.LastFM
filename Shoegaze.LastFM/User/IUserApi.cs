using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.User;

public enum TimePeriod
{
  Overall,
  SevenDay,
  OneMonth,
  ThreeMonth,
  SixMonth,
  TwelveMonth
}

/// <summary>
/// Access to user-related Last.fm API methods.
/// </summary>
public interface IUserApi
{
  /// <summary>
  /// Gets info for the current session user or for the specified username.
  /// </summary>
  /// <param name="username">Optional username to look up. If null, uses the authenticated session.</param>
  Task<ApiResult<UserInfo>> GetInfoAsync(string? username = null, CancellationToken ct = default);

  /// <summary>
  /// Gets a list of a user's Last.fm friends.
  /// </summary>
  /// <param name="username">Optional username. If null, uses the authenticated session.</param>
  /// <param name="page">Page number (optional).</param>
  /// <param name="limit">Number of results per page (optional).</param>
  /// <param name="includeRecentTracks">Whether to include recent track data per user.</param>
  /// <param name="ct">Cancellation token.</param>
  Task<ApiResult<PagedResult<UserInfo>>> GetFriendsAsync(
      string? username = null,
      bool includeRecentTracks = false,
      int? page = null,
      int? limit = null,
      CancellationToken ct = default);

  Task<ApiResult<PagedResult<TrackInfo>>> GetLovedTracksAsync(
    string? username = null,
    int? page = null,
    int? limit = null,
    CancellationToken ct = default);

  Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(
    string? username = null,
    TimePeriod? period = null, // e.g. "7day"
    int? limit = null,
    int? page = null,
    CancellationToken ct = default);

  Task<ApiResult<PagedResult<TrackInfo>>> GetRecentTracksAsync(string? username = null, bool? extended = null, DateTime? fromDate = null, DateTime? toDate = null, int? limit = null, int? page = null, CancellationToken ct = default);

  Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsAsync(
    string? username = null,
    int? limit = null,
    CancellationToken ct = default);

  Task<ApiResult<IReadOnlyList<T>>> GetPersonalTagsAsync<T>(string username, string tag, int? limit = null, int? page = null, CancellationToken ct = default) where T : ITagable;

  Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(string username, TimePeriod? period = null, int? limit = null, int? page = null, CancellationToken ct = default);

  Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsAsync(string username, TimePeriod? period = null, int? limit = null, int? page = null, CancellationToken ct = default);

  Task<ApiResult<IReadOnlyList<WeeklyChartInfo>>> GetWeeklyChartListAsync(string username, CancellationToken ct = default);
  Task<ApiResult<IReadOnlyList<T>>> GetWeeklyChartAsync<T>(string username, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default) where T : IChartable;
}
