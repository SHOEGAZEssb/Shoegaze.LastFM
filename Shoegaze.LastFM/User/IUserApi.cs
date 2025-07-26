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
}
