namespace Shoegaze.LastFM.Authentication;

/// <summary>
/// Provides methods for authenticating users with Last.fm.
/// </summary>
public interface IAuthService
{
  /// <summary>
  /// Generates the URL the user must visit to authorize the app.
  /// </summary>
  Task<Uri> GetAuthorizationUrlAsync();

  /// <summary>
  /// Exchanges the token for a session key after user authorization.
  /// </summary>
  Task<AuthSession> GetSessionAsync(string token, string tokenSecret, string verifier);

  /// <summary>
  /// Fully authenticates the user by opening the browser, waiting for redirect, and exchanging token.
  /// </summary>
  Task<AuthSession> AuthenticateAsync(CancellationToken cancellationToken = default);
}