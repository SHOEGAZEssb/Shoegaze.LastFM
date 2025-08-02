using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;
using Shoegaze.LastFM.User;

namespace Shoegaze.LastFM;

/// <summary>
/// Main implementation of <see cref="ILastfmClient"/>.
/// </summary>
public class LastfmClient : ILastfmClient
{
  public IUserApi User { get; }
  public ITrackApi Track { get; }
  public ITagApi Tag { get; }

  private readonly LastfmApiInvoker _invoker;

  public LastfmClient(string apiKey, string apiSecret, HttpClient httpClient)
  {
    _invoker = new LastfmApiInvoker(apiKey, apiSecret, httpClient);

    User = new UserApi(_invoker);
    Track = new TrackApi(_invoker);
    Tag = new TagApi(_invoker);
  }

  public void SetSessionKey(string sessionKey)
  {
    _invoker.SessionKey = sessionKey;
  }
}
