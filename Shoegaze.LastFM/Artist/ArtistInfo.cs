using Shoegaze.LastFM.Tag;
using System.Text.Json;

namespace Shoegaze.LastFM.Artist
{
  public sealed class ArtistInfo
  {
    /// <summary>
    /// Name of this artist.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Mbid of this artist.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// </remarks>
    public string? Mbid { get; init; }

    /// <summary>
    /// Url of the last.fm page of this artist.
    /// </summary>
    public required Uri Url { get; init; }

    /// <summary>
    /// Images of this artist.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// </remarks>
    public required IReadOnlyDictionary<ImageSize, Uri> Images { get; init; }

    public bool? IsStreamable { get; init; }

    public bool? OnTour { get; init; }

    public int? Listeners { get; init; }

    public int? Plays { get; init; }

    public int? UserPlayCount { get; init; }

    /// <summary>
    /// List of similar artists.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// </remarks>
    public required IReadOnlyList<ArtistInfo> SimilarArtists { get; init; }

    /// <summary>
    /// List of tags of this artist.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// </remarks>
    public required IReadOnlyList<TagInfo> Tags { get; init; }

    public WikiInfo? Biography { get; init; }

    internal static ArtistInfo FromJson(JsonElement root)
    {
      var artist = root.TryGetProperty("artist", out var artistProp) ? artistProp : root;

      var name = artist.GetProperty("name").GetString() ?? "";
      var url = new Uri(artist.GetProperty("url").GetString() ?? "");

      var mbid = artist.TryGetProperty("mbid", out var mbidProp) ? mbidProp.GetString() : null;

      var images = new Dictionary<ImageSize, Uri>();
      if (artist.TryGetProperty("image", out var imageArray))
      {
        foreach (var image in imageArray.EnumerateArray())
        {
          var urlText = image.GetProperty("#text").GetString();
          var sizeText = image.GetProperty("size").GetString();
          if (!string.IsNullOrWhiteSpace(urlText) &&
              Enum.TryParse<ImageSize>(sizeText, true, out var size))
          {
            images[size] = new Uri(urlText);
          }
        }
      }

      var isStreamable = artist.TryGetProperty("streamable", out var streamableProp)
        ? streamableProp.GetString() == "1"
        : (bool?)null;

      var onTour = artist.TryGetProperty("ontour", out var ontourProp)
        ? ontourProp.GetString() == "1"
        : (bool?)null;

      int? listeners = null;
      int? plays = null;
      int? userPlayCount = null;
      if (artist.TryGetProperty("stats", out var stats))
      {
        if (stats.TryGetProperty("listeners", out var l) && int.TryParse(l.GetString(), out var parsedListeners))
          listeners = parsedListeners;

        if (stats.TryGetProperty("playcount", out var p) && int.TryParse(p.GetString(), out var parsedPlays))
          plays = parsedPlays;

        if (stats.TryGetProperty("userplaycount", out var up) && int.TryParse(up.GetString(), out var parsedUserPlayCount))
          userPlayCount = parsedUserPlayCount;
      }


      var similar = new List<ArtistInfo>();
      if (artist.TryGetProperty("similar", out var similarProp) &&
          similarProp.TryGetProperty("artist", out var similarArray))
      {
        foreach (var sim in similarArray.EnumerateArray())
        {
          similar.Add(FromJson(sim));
        }
      }

      var tags = new List<TagInfo>();
      if (artist.TryGetProperty("tags", out var tagsProp) &&
          tagsProp.TryGetProperty("tag", out var tagArray))
      {
        foreach (var tag in tagArray.EnumerateArray())
        {
          tags.Add(TagInfo.FromJson(tag));
        }
      }

      WikiInfo? bio = null;
      if (artist.TryGetProperty("bio", out var bioProp))
        bio = WikiInfo.FromJson(bioProp);

      return new ArtistInfo
      {
        Name = name,
        Url = url,
        Mbid = mbid,
        Images = images,
        IsStreamable = isStreamable,
        OnTour = onTour,
        Listeners = listeners,
        Plays = plays,
        UserPlayCount = userPlayCount,
        SimilarArtists = similar,
        Tags = tags,
        Biography = bio
      };
    }

  }
}