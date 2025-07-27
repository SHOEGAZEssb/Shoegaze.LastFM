using Shoegaze.LastFM.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.User
{
  public sealed class LovedTrack : TrackBase
  {
    public DateTime? Date { get; init; }

    internal static LovedTrack FromJson(JsonElement root)
    {
      var name = root.GetProperty("name").GetString() ?? "";
      var url = root.GetProperty("url").GetString() ?? "";

      var artistElement = root.GetProperty("artist");
      var artistName = artistElement.GetProperty("name").GetString() ?? "";
      var artistUrl = artistElement.GetProperty("url").GetString() ?? "";

      DateTime? parsedDate = null;
      if (root.TryGetProperty("date", out var dateElem) &&
          dateElem.TryGetProperty("uts", out var utsProp) &&
          long.TryParse(utsProp.GetString(), out var uts))
      {
        parsedDate = DateTimeOffset.FromUnixTimeSeconds(uts).DateTime;
      }

      var images = JsonHelper.ParseImageArray(root);
      return new LovedTrack
      {
        Name = name,
        Url = url,
        ArtistName = artistName,
        ArtistUrl = artistUrl,
        Date = parsedDate,
        Images = images
      };
    }
  }
}
