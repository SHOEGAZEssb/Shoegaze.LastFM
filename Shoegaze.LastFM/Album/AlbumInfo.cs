using System.Text.Json;

namespace Shoegaze.LastFM.Album
{
  public class AlbumInfo
  {
    public required string Artist { get; init; }
    public required string Title { get; init; }
    public required Uri Url { get; init; }
    public string? Mbid { get; init; }

    public Dictionary<ImageSize, Uri> Images { get; init; } = [];

    internal static AlbumInfo FromJson(JsonElement root)
    {
      return new AlbumInfo
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
