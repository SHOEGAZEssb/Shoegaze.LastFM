using Shoegaze.LastFM.Track;
using System.Text.Json;

namespace Shoegaze.LastFM.User
{
  public sealed class LovedTrack : TrackBase
  {
    public DateTime? Date { get; init; }

    internal static LovedTrack FromJson(JsonElement root)
    {
      DateTime? parsedDate = null;
      if (root.TryGetProperty("date", out var dateElem) &&
          dateElem.TryGetProperty("uts", out var utsProp) &&
          long.TryParse(utsProp.GetString(), out var uts))
      {
        parsedDate = DateTimeOffset.FromUnixTimeSeconds(uts).DateTime;
      }

      var baseTrack = ParseBaseTrack(root);
      return new LovedTrack
      {
        Name = baseTrack.Name,
        Url = baseTrack.Url,
        ArtistName = baseTrack.ArtistName,
        ArtistUrl = baseTrack.ArtistUrl,
        Images = baseTrack.Images,
        Date = parsedDate,
      };
    }
  }
}
