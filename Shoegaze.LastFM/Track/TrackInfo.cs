using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using System.Text.Json;

namespace Shoegaze.LastFM.Track
{
  public sealed class TrackInfo
  {
    /// <summary>
    /// Name of this track.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Mbid of this track.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// </remarks>
    public string? Mbid { get; set; }

    /// <summary>
    /// Url to the last.fm page of this track.
    /// </summary>
    public required Uri Url { get; set; }

    /// <summary>
    /// Duration of this track.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// </remarks>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Amount of listeners this track has.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// </remarks>
    public int? Listeners { get; set; }

    /// <summary>
    /// Amount of plays this track has.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// </remarks>
    public int? PlayCount { get; set; }

    /// <summary>
    /// Amount of plays the user has for which the request has been made.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.GetInfoByNameAsync(string, string, string?, bool, CancellationToken)"/> with included username.
    /// - <see cref="ITrackApi.GetInfoByMbidAsync(string, string?, CancellationToken)"/> with included username.
    /// - <see cref="User.IUserApi.GetTopTracksAsync(string?, User.TimePeriod?, int?, int?, CancellationToken)"/> with included username.
    /// </remarks>
    public int? UserPlayCount { get; set; }

    /// <summary>
    /// Indicates if the user for which the request has been made has loved this track.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.GetInfoByNameAsync(string, string, string?, bool, CancellationToken)"/> with included username.
    /// - <see cref="ITrackApi.GetInfoByMbidAsync(string, string?, CancellationToken)"/> with included username.
    /// - <see cref="User.IUserApi.GetLovedTracksAsync(string?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public bool? UserLoved { get; set; }

    /// <summary>
    /// Indicates the rank of a track when getting the top tracks for a user.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="User.IUserApi.GetTopTracksAsync(string?, User.TimePeriod?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public int? Rank { get; set; }

    public ArtistInfo? Artist { get; set; }
    public AlbumInfo? Album { get; set; }
    public WikiInfo? Wiki { get; set; }

    /// <summary>
    /// List of most used tags for this tracks.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.GetInfoByNameAsync(string, string, string?, bool, CancellationToken)"/>.
    /// - <see cref="ITrackApi.GetInfoByMbidAsync(string, string?, CancellationToken)"/>.
    /// </remarks>
    public IReadOnlyList<TagInfo>? TopTags { get; set; }

    /// <summary>
    /// List of images for this track.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// </remarks>
    public required IReadOnlyDictionary<ImageSize, Uri> Images { get; set; }

    /// <summary>
    /// The time the user for which the request has been made has played this track.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="User.IUserApi.GetRecentTracksAsync(string?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public DateTime? PlayedAt { get; set; }

    /// <summary>
    /// If the user for which the request has been made is currently listening to this track.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="User.IUserApi.GetRecentTracksAsync(string?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public bool? IsNowPlaying { get; set; }
    public bool? IsStreamable { get; set; }

    internal static TrackInfo FromJson(JsonElement root)
    {
      var track = root.TryGetProperty("track", out var trackProperty) ? trackProperty : root;

      var name = track.GetProperty("name").GetString() ?? "";
      var url = new Uri(track.GetProperty("url").GetString() ?? "");
      var mbid = track.TryGetProperty("mbid", out var mbidProp) ? mbidProp.GetString() : null;

      TimeSpan? duration = null;
      if (track.TryGetProperty("duration", out var durProp) && long.TryParse(durProp.GetString(), out var dur))
        duration = TimeSpan.FromMilliseconds(dur);

      bool? isStreamable = null;
      if (track.TryGetProperty("streamable", out var streamableProp))
      {
        if (streamableProp.ValueKind == JsonValueKind.Object)
        {
          if(streamableProp.TryGetProperty("#text", out var streamableText))
            isStreamable = streamableText.GetString() == "1";
        }
        else if (streamableProp.ValueKind == JsonValueKind.String)
          isStreamable = streamableProp.GetString() == "1";
      }
          

      int? listeners = null;
      if (track.TryGetProperty("listeners", out var listenersProp) && int.TryParse(listenersProp.GetString(), out var parsedListeners))
        listeners = parsedListeners;

      int? playcount = null;
      if (track.TryGetProperty("playcount", out var playProp) && int.TryParse(playProp.GetString(), out var parsedPlaycount))
        playcount = parsedPlaycount;

      int? userPlaycount = null;
      if (track.TryGetProperty("userplaycount", out var upProp) && int.TryParse(upProp.GetString(), out var parsedUp))
        userPlaycount = parsedUp;

      bool? isLoved = null;
      if (track.TryGetProperty("userloved", out var lovedProp))
        isLoved = lovedProp.GetString() == "1";

      int? rank = null;
      if(track.TryGetProperty("@attr", out var attrProp))
        rank = attrProp.TryGetProperty("rank", out var rankProp) ? int.Parse(rankProp.GetString()!) : null;

      ArtistInfo? artist = null;
      if(track.TryGetProperty("artist", out var artistProp))
      {
        artist = ArtistInfo.FromJson(artistProp);
      }

      AlbumInfo? album = null;
      if (track.TryGetProperty("album", out var albumProp))
      {
        album = AlbumInfo.FromJson(albumProp);
      }

      var tags = new List<TagInfo>();
      if (track.TryGetProperty("toptags", out var tagsProp) &&
          tagsProp.TryGetProperty("tag", out var tagArray))
      {
        foreach (var tag in tagArray.EnumerateArray())
        {
          tags.Add(TagInfo.FromJson(tag));
        }
      }

      WikiInfo? wiki = null;
      if (track.TryGetProperty("wiki", out var wikiProp))
        wiki = WikiInfo.FromJson(wikiProp);

      return new TrackInfo
      {
        Name = name,
        Mbid = mbid,
        Url = url,
        Duration = duration,
        IsStreamable = isStreamable,
        Listeners = listeners,
        PlayCount = playcount,
        UserPlayCount = userPlaycount,
        UserLoved = isLoved,
        Rank = rank,
        Artist = artist!,
        Album = album,
        TopTags = tags,
        Wiki = wiki,
        Images = JsonHelper.ParseImageArray(root)
      };
    }


    //public static TrackInfo FromJson(JsonElement root)
    //{
    //  var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";
    //  var url = root.TryGetProperty("url", out var urlProp) ? urlProp.GetString() ?? "" : "";

    //  ArtistInfo? artist = null;
    //  if (root.TryGetProperty("artist", out var artistProp))
    //  {
    //    artist = ArtistInfo.FromJson(artistProp);
    //    //if (artistProp.ValueKind == JsonValueKind.Object)
    //    //{
    //    //  artistName = artistProp.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
    //    //  artistUrl = artistProp.TryGetProperty("url", out var u) ? u.GetString() ?? "" : "";
    //    //}
    //    //else
    //    //{
    //    //  artistName = artistProp.GetString() ?? "";
    //    //}
    //  }

    //  DateTime? date = null;
    //  if (root.TryGetProperty("date", out var dateElem) &&
    //      dateElem.TryGetProperty("uts", out var utsProp) &&
    //      long.TryParse(utsProp.GetString(), out var uts))
    //  {
    //    date = DateTimeOffset.FromUnixTimeSeconds(uts).DateTime;
    //  }

    //  return new TrackInfo
    //  {
    //    Name = name,
    //    Url = new Uri(url),
    //    Images = JsonHelper.ParseImageArray(root),
    //    Date = date
    //  };
    //}
  }
}
