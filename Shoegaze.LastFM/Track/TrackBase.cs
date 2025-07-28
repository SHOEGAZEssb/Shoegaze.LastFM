using System.Text.Json;

namespace Shoegaze.LastFM.Track
{
  public abstract class TrackBase
  {
    public string Name { get; init; } = "";
    public string Url { get; init; } = "";
    public string ArtistName { get; init; } = "";
    public string ArtistUrl { get; init; } = "";
    public Dictionary<ImageSize, Uri> Images { get; init; } = [];

    public override string ToString() => $"{ArtistName} – {Name}";

    internal static TrackBase ParseBaseTrack(JsonElement root)
    {
      var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";
      var url = root.TryGetProperty("url", out var urlProp) ? urlProp.GetString() ?? "" : "";

      var artistName = "";
      var artistUrl = "";
      if (root.TryGetProperty("artist", out var artistProp))
      {
        if (artistProp.ValueKind == JsonValueKind.Object)
        {
          artistName = artistProp.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
          artistUrl = artistProp.TryGetProperty("url", out var u) ? u.GetString() ?? "" : "";
        }
        else
        {
          artistName = artistProp.GetString() ?? "";
        }
      }

      return new BasicTrack
      {
        Name = name,
        Url = url,
        ArtistName = artistName,
        ArtistUrl = artistUrl,
        Images = JsonHelper.ParseImageArray(root)
      };
    }

    // Helper class since TrackBase is abstract
    private sealed class BasicTrack : TrackBase { }
  }
}
