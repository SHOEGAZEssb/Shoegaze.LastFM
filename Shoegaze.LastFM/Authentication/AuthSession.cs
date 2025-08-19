namespace Shoegaze.LastFM.Authentication;

/// <summary>
/// Represents a Last.fm session after user authentication.
/// </summary>
public class AuthSession(string username, string sessionKey)
{
  /// <summary>
  /// The username of this authenticated session.
  /// </summary>
  public string Username { get; } = username;

  /// <summary>
  /// The sessions key off this authenticated session.
  /// </summary>
  public string SessionKey { get; } = sessionKey;
}