using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Chart;
using Shoegaze.LastFM.Geo;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;
using Shoegaze.LastFM.User;

namespace Shoegaze.LastFM;

/// <summary>
/// Grants access to last.fms api.
/// </summary>
public class LastfmClient : ILastfmClient
{
  public IUserApi User { get; }
  public ITrackApi Track { get; }
  public ITagApi Tag { get; }
  public IArtistApi Artist { get; }
  public IAlbumApi Album { get; }
  public IChartApi Chart { get; }
  public IGeoApi Geo { get; }

  private readonly LastfmApiInvoker _invoker;

  public LastfmClient(string apiKey, string apiSecret, HttpClient httpClient)
  {
    _invoker = new LastfmApiInvoker(apiKey, apiSecret, httpClient);

    User = new UserApi(_invoker);
    Track = new TrackApi(_invoker);
    Tag = new TagApi(_invoker);
    Artist = new ArtistApi(_invoker);
    Album = new AlbumApi(_invoker);
    Chart = new ChartApi(_invoker);
    Geo = new GeoApi(_invoker);
  }

  public void SetSessionKey(string sessionKey)
  {
    ArgumentException.ThrowIfNullOrEmpty(sessionKey);
    _invoker.SessionKey = sessionKey;
  }
}