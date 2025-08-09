namespace Shoegaze.LastFM.Authentication;

/// <summary>
/// Represents a Last.fm session after user authentication.
/// </summary>
public class AuthSession(string username, string sessionKey)
{
  public string Username { get; } = username;
  public string SessionKey { get; } = sessionKey;
}