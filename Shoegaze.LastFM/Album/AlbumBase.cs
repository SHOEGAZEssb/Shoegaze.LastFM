using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Album
{
  public class AlbumBase
  {
    public required string Artist { get; init; }
    public required string Title { get; init; }
    public required Uri Url { get; init; }
    public string? Mbid { get; init; }

    public Dictionary<ImageSize, string> Images { get; init; } = [];

    internal static AlbumBase FromJson(JsonElement root)
    {
      return new AlbumBase
      {
        Artist = root.GetProperty("artist").GetString() ?? "",
        Title = root.GetProperty("title").GetString() ?? "",
        Url = new Uri(root.GetProperty("url").GetString() ?? ""),
        Mbid = root.TryGetProperty("mbid", out var mbidProp) ? mbidProp.GetString() : null,
        Images = JsonHelper.ParseImageArray(root)
      };
    }
  }
}
