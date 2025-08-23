namespace Shoegaze.LastFM.Authentication;

/// <summary>
/// Provides methods for authenticating users with Last.fm.
/// </summary>
public interface IAuthService
{
  /// <summary>
  /// Generates the authorization URL that the user must visit to grant access.
  /// </summary>
  /// <param name="callbackUrl">The callback url to redirect to.</param>
  /// <returns>
  /// A <see cref="Uri"/> pointing to the Last.fm authentication page with the
  /// configured API key and callback URL.
  /// </returns>
  Task<Uri> GetAuthorizationUrlAsync(string callbackUrl);

  /// <summary>
  /// Exchanges a temporary token for a permanent Last.fm session.
  /// </summary>
  /// <param name="token">The request token received from Last.fm after user authorization.</param>
  /// <returns>
  /// An <see cref="AuthSession"/> containing the authorized user’s name and session key.
  /// </returns>
  Task<AuthSession> GetSessionAsync(string token);

  /// <summary>
  /// Starts the full authentication process, including opening the system browser,
  /// listening for the Last.fm callback, and exchanging the temporary token for a session key.
  /// </summary>
  /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
  /// <returns>
  /// An <see cref="AuthSession"/> containing the authorized user’s name and session key.
  /// </returns>
  Task<AuthSession> AuthenticateAsync(CancellationToken cancellationToken = default);
}