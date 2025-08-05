using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;
using Shoegaze.LastFM.User;

namespace Shoegaze.LastFM;

/// <summary>
/// High-level entry point to access Last.fm API features.
/// </summary>
public interface ILastfmClient
{
  /// <summary>
  /// Access to user-related API endpoints.
  /// </summary>
  IUserApi User { get; }

  /// <summary>
  /// Access to artist-related API endpoints.
  /// </summary>
  IArtistApi Artist { get; }

  /// <summary>
  /// Access to track-related API endpoints.
  /// </summary>
  ITrackApi Track { get; }

  /// <summary>
  /// Access to tag-related API endpoints.
  /// </summary>
  ITagApi Tag { get; }

  /// <summary>
  /// Sets or updates the session key for authenticated requests.
  /// </summary>
  void SetSessionKey(string sessionKey);
}