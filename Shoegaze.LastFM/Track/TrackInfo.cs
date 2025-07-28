using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Tag;
using System.Text.Json;

namespace Shoegaze.LastFM.Track
{
  public sealed class TrackInfo : TrackBase
  {
    public string? Mbid { get; init; }
    public int? DurationMs { get; init; }
    public int? Listeners { get; init; }
    public int? Playcount { get; init; }
    public bool IsStreamable { get; init; }
    public int? UserPlaycount { get; init; }
    public bool IsUserLoved { get; init; }

    public AlbumBase? Album { get; init; }
    public IReadOnlyList<TagInfo> TopTags { get; init; } = [];
    public WikiInfo? Wiki { get; init; }

    public static TrackInfo FromJson(JsonElement root)
    {
      var baseTrack = ParseBaseTrack(root);

      var mbid = root.TryGetProperty("mbid", out var mbidProp) ? mbidProp.GetString() : null;

      var durationMs = root.TryGetProperty("duration", out var durProp) && int.TryParse(durProp.GetString(), out var dur) ? dur : (int?)null;
      var listeners = root.TryGetProperty("listeners", out var lisProp) && int.TryParse(lisProp.GetString(), out var lis) ? lis : (int?)null;
      var playcount = root.TryGetProperty("playcount", out var pcProp) && int.TryParse(pcProp.GetString(), out var pc) ? pc : (int?)null;

      var streamable = root.TryGetProperty("streamable", out var streamProp) &&
                       streamProp.TryGetProperty("#text", out var stText) &&
                       stText.GetString() == "1";

      var userPlayCount = root.TryGetProperty("userplaycount", out var upcProp) && int.TryParse(upcProp.GetString(), out var upc) ? upc : (int?)null;
      var isLoved = root.TryGetProperty("userloved", out var lovedProp) && lovedProp.GetString() == "1";

      AlbumBase? album = null;
      if (root.TryGetProperty("album", out var albumProp))
      {
        album = AlbumBase.FromJson(albumProp);
      }

      var tags = new List<TagInfo>();
      if (root.TryGetProperty("toptags", out var tagContainer) && tagContainer.TryGetProperty("tag", out var tagArr))
      {
        if (tagArr.ValueKind == JsonValueKind.Array)
        {
          tags.AddRange(tagArr.EnumerateArray().Select(TagInfo.FromJson));
        }
        else if (tagArr.ValueKind == JsonValueKind.Object)
        {
          tags.Add(TagInfo.FromJson(tagArr));
        }
      }

      WikiInfo? wiki = null;
      if (root.TryGetProperty("wiki", out var wikiProp))
      {
        wiki = WikiInfo.FromJson(wikiProp);
      }

      return new TrackInfo
      {
        Name = baseTrack.Name,
        Url = baseTrack.Url,
        ArtistName = baseTrack.ArtistName,
        ArtistUrl = baseTrack.ArtistUrl,
        Images = baseTrack.Images,

        Mbid = mbid,
        DurationMs = durationMs,
        Listeners = listeners,
        Playcount = playcount,
        IsStreamable = streamable,
        UserPlaycount = userPlayCount,
        IsUserLoved = isLoved,
        Album = album,
        TopTags = tags,
        Wiki = wiki
      };
    }
  }
}