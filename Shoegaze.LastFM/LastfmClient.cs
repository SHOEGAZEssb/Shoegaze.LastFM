using Shoegaze.LastFM.Track;
using Shoegaze.LastFM.User;

namespace Shoegaze.LastFM;

/// <summary>
/// Main implementation of <see cref="ILastfmClient"/>.
/// </summary>
public class LastfmClient : ILastfmClient
{
  private readonly LastfmApiInvoker _invoker;

  public LastfmClient(string apiKey, string apiSecret, HttpClient httpClient)
  {
    _invoker = new LastfmApiInvoker(apiKey, apiSecret, httpClient);

    User = new UserApi(_invoker);
    //Artist = new ArtistApi(_invoker);
    Track = new TrackApi(_invoker);
  }

  public IUserApi User { get; }
  //public IArtistApi Artist { get; }
  public ITrackApi Track { get; }

  public void SetSessionKey(string sessionKey)
  {
    _invoker.SessionKey = sessionKey;
  }
}
