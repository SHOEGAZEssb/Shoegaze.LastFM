using Shoegaze.LastFM.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.User
{
  public sealed class RecentTrack : TrackBase
  {
    public string ArtistMbid { get; set; } = "";
    public string AlbumName { get; set; } = "";
    public string AlbumMbid { get; set; } = "";
    public DateTime? PlayedAt { get; init; }
    public bool IsNowPlaying { get; init; }
    public bool IsStreamable { get; set; }

    public static RecentTrack FromJson(JsonElement root)
    {
      var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";
      var url = root.TryGetProperty("url", out var urlProp) ? urlProp.GetString() ?? "" : "";

      // Parse artist
      var artistName = "";
      var artistUrl = "";
      var artistMbid = "";
      if (root.TryGetProperty("artist", out var artistProp))
      {
        if (artistProp.ValueKind == JsonValueKind.Object)
        {
          artistName = artistProp.TryGetProperty("#text", out var n) ? n.GetString() ?? "" : "";
          artistMbid = artistProp.TryGetProperty("mbid", out var m) ? m.GetString() ?? "" : "";
        }
        else if (artistProp.ValueKind == JsonValueKind.String)
        {
          artistName = artistProp.GetString() ?? "";
        }
      }

      // Parse album (optional)
      var albumName = "";
      var albumMbid = "";
      if (root.TryGetProperty("album", out var albumProp))
      {
        if (albumProp.ValueKind == JsonValueKind.Object)
        {
          albumName = albumProp.TryGetProperty("#text", out var n) ? n.GetString() ?? "" : "";
          albumMbid = albumProp.TryGetProperty("mbid", out var m) ? m.GetString() ?? "" : "";
        }
        else if (albumProp.ValueKind == JsonValueKind.String)
        {
          albumName = albumProp.GetString() ?? "";
        }
      }

      // Check if now playing
      var isNowPlaying = root.TryGetProperty("@attr", out var attrProp) &&
                         attrProp.TryGetProperty("nowplaying", out var nowPlayingProp) &&
                         nowPlayingProp.GetString() == "true";

      // Parse play date if available
      DateTime? playedAt = null;
      if (root.TryGetProperty("date", out var dateProp) &&
          dateProp.TryGetProperty("uts", out var utsProp) &&
          long.TryParse(utsProp.GetString(), out var uts))
      {
        playedAt = DateTimeOffset.FromUnixTimeSeconds(uts).DateTime;
      }

      // Parse streamable
      var isStreamable = root.TryGetProperty("streamable", out var streamProp) &&
                         streamProp.GetString() == "1";

      return new RecentTrack
      {
        Name = name,
        Url = url,
        ArtistName = artistName,
        ArtistMbid = artistMbid,
        AlbumName = albumName,
        AlbumMbid = albumMbid,
        Images = JsonHelper.ParseImageArray(root),
        IsNowPlaying = isNowPlaying,
        PlayedAt = playedAt,
        IsStreamable = isStreamable
      };
    }
  }

}
