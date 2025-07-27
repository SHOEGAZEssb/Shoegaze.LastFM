namespace Shoegaze.LastFM.User;

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
      int? page = null,
      int? limit = null,
      bool includeRecentTracks = false,
      CancellationToken ct = default);

  Task<ApiResult<PagedResult<LovedTrack>>> GetLovedTracksAsync(
    string? username = null,
    int? page = null,
    int? limit = null,
    CancellationToken ct = default);
}
