using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Chart;
using Shoegaze.LastFM.Geo;
using Shoegaze.LastFM.Library;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;
using Shoegaze.LastFM.User;

namespace Shoegaze.LastFM;

/// <summary>
/// Grants access to last.fms api.
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
  /// Access to album-related API endpoints.
  /// </summary>
  IAlbumApi Album { get; }

  /// <summary>
  /// Access to chart-related API endpoints.
  /// </summary>
  IChartApi Chart { get; }

  /// <summary>
  /// Access to geo-related API endpoints.
  /// </summary>
  IGeoApi Geo { get; }

  /// <summary>
  /// Access to library-related API endpoints.
  /// </summary>
  ILibraryApi Library { get; }

  /// <summary>
  /// Sets or updates the session key for authenticated requests.
  /// </summary>
  void SetSessionKey(string sessionKey);
}