using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.User;
using System.Text.Json;

namespace Shoegaze.LastFM.Track
{
  /// <summary>
  /// A last.fm track.
  /// </summary>
  public sealed class TrackInfo : IUserChartable, IUserTagable, IJsonDeserializable<TrackInfo>
  {
    /// <summary>
    /// Name of this track.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Mbid of this track.
    /// </summary>
    /// <remarks>
    /// May be null or empty.
    /// </remarks>
    public string? Mbid { get; private set; }

    /// <summary>
    /// Url to the last.fm page of this track.
    /// </summary>
    public required Uri Url { get; set; }

    /// <summary>
    /// Duration of this track.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// </remarks>
    public TimeSpan? Duration { get; private set; }

    /// <summary>
    /// Amount of listeners this track has.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.GetInfoByNameAsync(string, string, string?, bool, CancellationToken)"/>.
    /// - <see cref="ITrackApi.GetInfoByMbidAsync(string, string?, CancellationToken)"/>.
    /// - <see cref="ITrackApi.SearchAsync(string, string?, int?, int?, CancellationToken)"/>
    /// - <see cref="IArtistApi.GetTopTracksByNameAsync(string, bool, int?, int?, CancellationToken)"/>
    /// - <see cref="IArtistApi.GetTopTracksByMbidAsync(string, int?, int?, CancellationToken)"/>
    /// </remarks>
    public int? ListenerCount { get; private set; }

    /// <summary>
    /// Amount of plays this track has.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="IArtistApi.GetTopTracksByNameAsync(string, bool, int?, int?, CancellationToken)"/>
    /// - <see cref="IArtistApi.GetTopTracksByMbidAsync(string, int?, int?, CancellationToken)"/>
    /// </remarks>
    public int? PlayCount { get; internal set; }

    /// <summary>
    /// Amount of plays the user has for which the request has been made.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.GetInfoByNameAsync(string, string, string?, bool, CancellationToken)"/> with included username.
    /// - <see cref="ITrackApi.GetInfoByMbidAsync(string, string?, CancellationToken)"/> with included username.
    /// - <see cref="User.IUserApi.GetTopTracksAsync(string?, User.TimePeriod?, int?, int?, CancellationToken)"/> with included username.
    /// </remarks>
    public int? UserPlayCount { get; internal set; }

    /// <summary>
    /// Indicates if the user for which the request has been made has loved this track.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.GetInfoByNameAsync(string, string, string?, bool, CancellationToken)"/> with included username.
    /// - <see cref="ITrackApi.GetInfoByMbidAsync(string, string?, CancellationToken)"/> with included username.
    /// - <see cref="User.IUserApi.GetLovedTracksAsync(string?, int?, int?, CancellationToken)"/>.
    /// - <see cref="User.IUserApi.GetRecentTracksAsync(string?, bool?, DateTimeOffset?, DateTimeOffset?, bool, int?, int?, CancellationToken)"/> with extended = true.
    /// </remarks>
    public bool? UserLoved { get; internal set; }

    /// <summary>
    /// The time when the user for which the request has been made has loved this track.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="User.IUserApi.GetLovedTracksAsync(string?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public DateTime? UserLovedDate { get; private set; }

    /// <summary>
    /// Indicates the match score of a track for which similar
    /// tracks have been requested.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.GetSimilarByNameAsync(string, string, bool, int?, CancellationToken)"/>.
    /// </remarks>
    public double? Match { get; private set; }

    /// <summary>
    /// Info about the artist of this track.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.GetInfoByNameAsync(string, string, string?, bool, CancellationToken)"/>.
    /// - <see cref="ITrackApi.GetInfoByMbidAsync(string, string?, CancellationToken)"/>.
    /// - <see cref="IArtistApi.GetTopTracksByNameAsync(string, bool, int?, int?, CancellationToken)"/>
    /// - <see cref="IArtistApi.GetTopTracksByMbidAsync(string, int?, int?, CancellationToken)"/>
    /// </remarks>
    public ArtistInfo? Artist { get; private set; }

    /// <summary>
    /// Info about the album this track is on.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.GetInfoByNameAsync(string, string, string?, bool, CancellationToken)"/>.
    /// - <see cref="ITrackApi.GetInfoByMbidAsync(string, string?, CancellationToken)"/>.
    /// </remarks>
    public AlbumInfo? Album { get; private set; }

    /// <summary>
    /// The wiki of this track.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.GetInfoByNameAsync(string, string, string?, bool, CancellationToken)"/>.
    /// - <see cref="ITrackApi.GetInfoByMbidAsync(string, string?, CancellationToken)"/>.
    /// </remarks>
    public WikiInfo? Wiki { get; private set; }

    /// <summary>
    /// List of most used tags for this tracks.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.GetInfoByNameAsync(string, string, string?, bool, CancellationToken)"/>.
    /// - <see cref="ITrackApi.GetInfoByMbidAsync(string, string?, CancellationToken)"/>.
    /// </remarks>
    public IReadOnlyList<TagInfo>? TopTags { get; private set; }

    /// <summary>
    /// List of images for this track.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// </remarks>
    public IReadOnlyDictionary<ImageSize, Uri> Images { get; private set; } = new Dictionary<ImageSize, Uri>();

    /// <summary>
    /// The time the user for which the request has been made has played this track.
    /// In UTC timezone.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="IUserApi.GetRecentTracksAsync(string?, bool?, DateTimeOffset?, DateTimeOffset?, bool, int?, int?, CancellationToken)"/> except when <see cref="IsNowPlaying"/> is true.
    /// </remarks>
    public DateTime? PlayedAtUtc { get; private set; }

    /// <summary>
    /// If the user for which the request has been made is currently listening to this track.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="IUserApi.GetRecentTracksAsync(string?, bool?, DateTimeOffset?, DateTimeOffset?, bool, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public bool IsNowPlaying { get; private set; }

    /// <summary>
    /// If the track can be streamed / previewed on the last.fm website.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="ITrackApi.SearchAsync(string, string?, int?, int?, CancellationToken)"/>.
    /// - <see cref="IArtistApi.GetTopTracksByNameAsync(string, bool, int?, int?, CancellationToken)"/>
    /// - <see cref="IArtistApi.GetTopTracksByMbidAsync(string, int?, int?, CancellationToken)"/>
    /// </remarks>
    public bool? IsStreamable { get; private set; }

    internal static TrackInfo FromJson(JsonElement root)
    {
      var track = root.TryGetProperty("track", out var trackProperty) ? trackProperty : root;

      var name = track.GetProperty("name").GetString() ?? "";
      var url = new Uri(track.GetProperty("url").GetString() ?? "");
      var mbid = track.TryGetProperty("mbid", out var mbidProp) ? mbidProp.GetString() : null;

      TimeSpan? duration = null;
      if (track.TryGetProperty("duration", out var durProp) && JsonHelper.TryParseNumber<long>(durProp, out var dur))
        duration = TimeSpan.FromMilliseconds(dur);

      bool? isStreamable = null;
      if (track.TryGetProperty("streamable", out var streamableProp))
      {
        if (streamableProp.ValueKind == JsonValueKind.Object)
        {
          if (streamableProp.TryGetProperty("#text", out var streamableText))
            isStreamable = streamableText.GetString() == "1";
        }
        else if (streamableProp.ValueKind == JsonValueKind.String)
          isStreamable = streamableProp.GetString() == "1";
      }

      int? listeners = null;
      if (track.TryGetProperty("listeners", out var listenersProp) && JsonHelper.TryParseNumber<int>(listenersProp, out var parsedListeners))
        listeners = parsedListeners;

      int? playcount = null;
      if (track.TryGetProperty("playcount", out var playProp) && JsonHelper.TryParseNumber<int>(playProp, out var parsedPlaycount))
        playcount = parsedPlaycount;

      int? userPlaycount = null;
      if (track.TryGetProperty("userplaycount", out var upProp) && JsonHelper.TryParseNumber<int>(upProp, out var parsedUp)
          || track.TryGetProperty("playcount", out upProp) && JsonHelper.TryParseNumber<int>(upProp, out parsedUp))
        userPlaycount = parsedUp;

      bool? isLoved = null;
      if (track.TryGetProperty("userloved", out var lovedProp)
          || track.TryGetProperty("loved", out lovedProp))
        isLoved = lovedProp.GetString() == "1";

      DateTime? date = null;
      if (track.TryGetProperty("date", out var dateProp) && dateProp.TryGetProperty("uts", out var dateText))
          date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(dateText.GetString()!, System.Globalization.CultureInfo.InvariantCulture)).UtcDateTime;

      bool? nowPlaying = null;
      if (track.TryGetProperty("@attr", out var attrProp))
        nowPlaying = attrProp.TryGetProperty("nowplaying", out var nowPlayingProp) ? (bool.TryParse(nowPlayingProp.GetString(), out var nowPlayingValue) ? nowPlayingValue : null) : null;

      double? match = null;
      if (track.TryGetProperty("match", out var matchProp) && JsonHelper.TryParseNumber<double>(matchProp, out var matchValue))
        match = matchValue;

      ArtistInfo? artist = null;
      if (track.TryGetProperty("artist", out var artistProp))
      {
        artist = ArtistInfo.FromJson(artistProp);
      }

      AlbumInfo? album = null;
      if (track.TryGetProperty("album", out var albumProp))
      {
        // check for empty album property
        if (albumProp.TryGetProperty("#text", out var albumTextProperty) && string.IsNullOrEmpty(albumTextProperty.GetString()))
          album = null;
        else
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
        ListenerCount = listeners,
        UserPlayCount = userPlaycount,
        UserLoved = isLoved,
        IsNowPlaying = nowPlaying ?? false,
        PlayedAtUtc = date, // playedat is only available in user.getRecentTracks, in which case album must be available (only when not IsNowPlaying)
        UserLovedDate = album == null ? date : null, // userloveddate is only available in user.getLovedTracks, in which case album is null
        PlayCount = playcount, // user playcount may also be called "playcount" when using user.GetTopTtracks, so if rank is available the "playcount" is actually the userplaycount
        Match = match,
        Artist = artist!,
        Album = album,
        TopTags = tags,
        Wiki = wiki,
        Images = JsonHelper.ParseImageArray(root)
      };
    }
  }
}