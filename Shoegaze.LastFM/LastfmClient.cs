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
public class LastfmClient : ILastfmClient
{
  #region Properties

  /// <summary>
  /// Access to user-related api endpoints.
  /// </summary>
  public IUserApi User { get; }

  /// <summary>
  /// Access to track-related api endpoints.
  /// </summary>
  public ITrackApi Track { get; }

  /// <summary>
  /// Access to tag-related api endpoints.
  /// </summary>
  public ITagApi Tag { get; }

  /// <summary>
  /// Access to artist-related api endpoints.
  /// </summary>
  public IArtistApi Artist { get; }

  /// <summary>
  /// Access to album-related api endpoints.
  /// </summary>
  public IAlbumApi Album { get; }

  /// <summary>
  /// Access to chart-related api endpoints.
  /// </summary>
  public IChartApi Chart { get; }

  /// <summary>
  /// Access to geo-related api endpoints.
  /// </summary>
  public IGeoApi Geo { get; }

  /// <summary>
  /// Access to library-related api endpoints.
  /// </summary>
  public ILibraryApi Library { get; }

  /// <summary>
  /// Indicates if this client can make
  /// authenticated requests.
  /// </summary>
  /// <remarks>
  /// This only checks if a session key has been set.
  /// It does not check that key for validity.
  /// </remarks>
  public bool IsAuthenticated => !string.IsNullOrEmpty(_invoker.SessionKey);

  private readonly LastfmApiInvoker _invoker;

  #endregion Properties

  /// <summary>
  /// Contructor.
  /// </summary>
  /// <param name="apiKey">Api key.</param>
  /// <param name="apiSecret">Api secret.</param>
  /// <param name="httpClient">Http client to use.</param>
  public LastfmClient(string apiKey, string apiSecret, HttpClient? httpClient = null)
  {
    _invoker = new LastfmApiInvoker(apiKey, apiSecret, httpClient);

    User = new UserApi(_invoker);
    Track = new TrackApi(_invoker);
    Tag = new TagApi(_invoker);
    Artist = new ArtistApi(_invoker);
    Album = new AlbumApi(_invoker);
    Chart = new ChartApi(_invoker);
    Geo = new GeoApi(_invoker);
    Library = new LibraryApi(_invoker);
  }

  /// <summary>
  /// Sets the sessions key for making authenticated requests.
  /// </summary>
  /// <param name="sessionKey">The session key.</param>
  /// <exception cref="ArgumentNullException">If <paramref name="sessionKey"/> is null or empty.</exception>
  public void SetSessionKey(string sessionKey)
  {
    ArgumentException.ThrowIfNullOrEmpty(sessionKey);
    _invoker.SessionKey = sessionKey;
  }
}