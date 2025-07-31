using Shoegaze.LastFM.Tag;
using System.Text.Json;

namespace Shoegaze.LastFM.Artist
{
  public sealed class ArtistInfo : IChartable, ITagable, IJsonDeserializable<ArtistInfo>
  {
    /// <summary>
    /// Name of this artist.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Mbid of this artist.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// </remarks>
    public string? Mbid { get; set; }

    /// <summary>
    /// Url of the last.fm page of this artist.
    /// </summary>
    public required Uri Url { get; set; }

    /// <summary>
    /// Images of this artist.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// </remarks>
    public required IReadOnlyDictionary<ImageSize, Uri> Images { get; set; } = new Dictionary<ImageSize, Uri>();

    public bool? IsStreamable { get; set; }

    public bool? OnTour { get; set; }

    public int? Listeners { get; set; }

    public int? PlayCount { get; set; }

    /// <summary>
    /// Amount of plays of this artist the user has for which the request has been made.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="User.IUserApi.GetTopArtistsAsync(string, User.TimePeriod?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public int? UserPlayCount { get; set; }

    /// <summary>
    /// Indicates the rank of an artist when getting the top artists for a user.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="User.IUserApi.GetTopArtistsAsync(string, User.TimePeriod?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public int? Rank { get; set; }

    /// <summary>
    /// List of similar artists.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// </remarks>
    public required IReadOnlyList<ArtistInfo> SimilarArtists { get; set; } = [];

    /// <summary>
    /// List of tags of this artist.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// </remarks>
    public required IReadOnlyList<TagInfo> Tags { get; set; } = [];

    public WikiInfo? Biography { get; set; }

    internal static ArtistInfo FromJson(JsonElement root)
    {
      var artist = root.TryGetProperty("artist", out var artistProp) ? artistProp : root;

      if (artist.ValueKind == JsonValueKind.String)
      {
        var artistName = artist.GetString() ?? "";
        return new ArtistInfo
        {
          Name = artistName,
          Url = UriHelper.MakeArtistUri(artistName)!,
          Images = new Dictionary<ImageSize, Uri>(),
          SimilarArtists = [],
          Tags = []
        };
      }

      // name might either be in the name, artist or #text property
      var name = artist.TryGetProperty("name", out var nameProp)
        ? nameProp.GetString() ?? ""
        : (artist.GetProperty("#text").GetString() ?? "");

      var url = artist.TryGetProperty("url", out var urlProp) ? new Uri(urlProp.GetString()!) : UriHelper.MakeArtistUri(name);

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

      int? rank = null;
      if (artist.TryGetProperty("@attr", out var attributeProp) && attributeProp.TryGetProperty("rank", out var rankProp) && int.TryParse(rankProp.GetString()!, out var rankNum))
        rank = rankNum;

      int? listeners = null;
      int? plays = null;
      //int? userPlayCount = null;
      if (artist.TryGetProperty("stats", out var stats))
      {
        if (stats.TryGetProperty("listeners", out var l) && int.TryParse(l.GetString(), out var parsedListeners))
          listeners = parsedListeners;

        if (stats.TryGetProperty("playcount", out var p) && int.TryParse(p.GetString(), out var parsedPlays))
          plays = parsedPlays;

        //if (stats.TryGetProperty("userplaycount", out var up) && int.TryParse(up.GetString(), out var parsedUserPlayCount))
        //  userPlayCount = parsedUserPlayCount;
      }
      else if (artist.TryGetProperty("playcount", out var playCountProp) && int.TryParse(playCountProp.GetString()!, out var playCount))
        plays = playCount;

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
        Url = url!,
        Mbid = mbid,
        Images = images,
        IsStreamable = isStreamable,
        OnTour = onTour,
        Listeners = listeners,
        Rank = rank,
        PlayCount = rank == null ? plays : null, // if rank is null, playcount property indicates global plays, otherwise userplaycount
        UserPlayCount = rank == null ? null : plays, // if rank is null, playcount property indicates global plays, otherwise userplaycount
        SimilarArtists = similar,
        Tags = tags,
        Biography = bio
      };
    }

  }
}