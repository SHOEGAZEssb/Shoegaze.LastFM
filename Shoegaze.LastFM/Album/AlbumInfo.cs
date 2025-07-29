using Shoegaze.LastFM.Artist;
using System.Text.Json;

namespace Shoegaze.LastFM.Album
{
  public class AlbumInfo
  {
    public ArtistInfo? Artist { get; set; }
    public required string Title { get; set; }
    public required Uri? Url { get; set; }
    public string? Mbid { get; set; }

    public Dictionary<ImageSize, Uri> Images { get; set; } = [];

    internal static AlbumInfo FromJson(JsonElement root)
    {
      var album = root.TryGetProperty("album", out var albumProp) ? albumProp : root;

      // name might either be in the name or #text property
      var name = album.TryGetProperty("title", out var nameProp)
        ? nameProp.GetString() ?? ""
        : (album.GetProperty("#text").GetString() ?? "");

      ArtistInfo? artist = album.TryGetProperty("artist", out var artistProp) ? ArtistInfo.FromJson(root) : null;

      return new AlbumInfo
      {
        Artist = artist,
        Title = name,
        Url = album.TryGetProperty("url", out var urlProperty) ? new Uri(urlProperty.GetString() ?? "") : (artist != null ? UriHelper.MakeAlbumUri(artist.Name, name) : null),
        Mbid = root.TryGetProperty("mbid", out var mbidProp) ? mbidProp.GetString() : null,
        Images = JsonHelper.ParseImageArray(root)
      };
    }
  }
}
