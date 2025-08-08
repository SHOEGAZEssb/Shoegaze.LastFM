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
    public IReadOnlyDictionary<ImageSize, Uri> Images { get; set; } = new Dictionary<ImageSize, Uri>();

    public bool? IsStreamable { get; set; }

    /// <summary>
    /// If this band is currently on tour.
    /// Usually this means this band has events that
    /// are available on last.fm.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="IArtistApi.GetInfoByNameAsync(string, string?, bool, CancellationToken)"/>
    /// - <see cref="IArtistApi.GetInfoByMbidAsync(string, string?, bool, CancellationToken)"/>
    /// </remarks>
    public bool? OnTour { get; set; }

    public int? ListenerCount { get; set; }

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
    /// Indicates the match score of an artist for which similar
    /// artists have been requested.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="IArtistApi.GetSimilarByNameAsync(string, bool, int?, CancellationToken)"/>.
    /// - <see cref="IArtistApi.GetSimilarByMbidAsync(string, bool, int?, CancellationToken)"/>.
    /// </remarks>
    public double? Match { get; set; }

    /// <summary>
    /// List of similar artists.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// </remarks>
    public IReadOnlyList<ArtistInfo> SimilarArtists { get; set; } = [];

    /// <summary>
    /// List of tags of this artist.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// </remarks>
    public IReadOnlyList<TagInfo> Tags { get; set; } = [];

    public WikiInfo? Biography { get; set; }

    internal static ArtistInfo FromJson(JsonElement root)
    {
      var artist = root.ValueKind == JsonValueKind.Object ?
        (root.TryGetProperty("artist", out var artistProp) ? artistProp : root) : root;

      if (artist.ValueKind == JsonValueKind.String)
      {
        var artistName = artist.GetString()!;
        return new ArtistInfo
        {
          Name = artistName,
          Url = UriHelper.MakeArtistUri(artistName)!,
        };
      }

      // name might either be in the name, artist or #text property
      var name = artist.TryGetProperty("name", out var nameProp)
        ? nameProp.GetString() ?? ""
        : (artist.GetProperty("#text").GetString()!);

      var url = artist.TryGetProperty("url", out var urlProp) ? new Uri(urlProp.GetString()!) : UriHelper.MakeArtistUri(name);

      var mbid = artist.TryGetProperty("mbid", out var mbidProp) ? mbidProp.GetString() : null;

      var images = JsonHelper.ParseImageArray(root);

      var isStreamable = artist.TryGetProperty("streamable", out var streamableProp)
        ? streamableProp.GetString() == "1"
        : (bool?)null;

      var onTour = artist.TryGetProperty("ontour", out var ontourProp)
        ? ontourProp.GetString() == "1"
        : (bool?)null;

      int? listeners = null;
      int? plays = null;
      int? userPlayCount = null;

      var statsProperty = artist.TryGetProperty("stats", out var stats) ? stats : artist; // stats may be nested inside a stats object
      if (statsProperty.TryGetProperty("listeners", out var l) && JsonHelper.TryParseNumber<int>(l, out var parsedListeners))
        listeners = parsedListeners;
      if (statsProperty.TryGetProperty("playcount", out var p) && JsonHelper.TryParseNumber<int>(p, out var parsedPlays))
        plays = parsedPlays;
      if (statsProperty.TryGetProperty("userplaycount", out var up) && JsonHelper.TryParseNumber<int>(up, out var parsedUserPlays))
        userPlayCount = parsedUserPlays;

      double? match = null;
      if (artist.TryGetProperty("match", out var matchProp) && JsonHelper.TryParseNumber<double>(matchProp, out var matchValue))
        match = matchValue;

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
        ListenerCount = listeners,
        PlayCount = plays,
        UserPlayCount = userPlayCount ?? plays,
        Match = match,
        SimilarArtists = similar,
        Tags = tags,
        Biography = bio
      };
    }

  }
}