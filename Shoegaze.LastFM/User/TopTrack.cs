using Shoegaze.LastFM.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.User
{
  public enum TimePeriod
  {
    Overall,
    SevenDay,
    OneMonth,
    ThreeMonth,
    SixMonth,
    TwelveMonth
  }

  public sealed class TopTrack : TrackBase
  {
    public int Playcount { get; init; }
    public int Rank { get; init; }
    public TimeSpan? Duration { get; init; }

    public static TopTrack FromJson(JsonElement root)
    {
      var baseTrack = ParseBaseTrack(root);

      return new TopTrack
      {
        Name = baseTrack.Name,
        Url = baseTrack.Url,
        ArtistName = baseTrack.ArtistName,
        ArtistUrl = baseTrack.ArtistUrl,
        Images = baseTrack.Images,
        Playcount = int.TryParse(root.GetProperty("playcount").GetString(), out var pc) ? pc : 0,
        Rank = int.TryParse(root.GetProperty("@attr").GetProperty("rank").GetString(), out var r) ? r : 0,
        Duration = int.TryParse(root.GetProperty("duration").GetString(), out var d) ? TimeSpan.FromSeconds(d) : null
      };
    }
  }

}
