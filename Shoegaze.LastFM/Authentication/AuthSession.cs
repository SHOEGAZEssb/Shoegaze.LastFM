namespace Shoegaze.LastFM.Authentication;

/// <summary>
/// Represents a Last.fm session after user authentication.
/// </summary>
public class AuthSession
{
  public string Username { get; set; } = default!;
  public string SessionKey { get; set; } = default!;
}