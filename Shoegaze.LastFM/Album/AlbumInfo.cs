using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;
using Shoegaze.LastFM.User;
using System.Text.Json;

namespace Shoegaze.LastFM.Album
{
  /// <summary>
  /// Object with info about an album, fetched from last.fm
  /// </summary>
  public class AlbumInfo : IUserChartable, IUserTagable, IJsonDeserializable<AlbumInfo>
  {
    /// <summary>
    /// Name of this artist.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Url of this album.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="IAlbumApi.GetInfoByNameAsync(string, string, string?, bool, string?, CancellationToken)"/>.
    /// - <see cref="IAlbumApi.GetInfoByMbidAsync(string, string?, bool, string?, CancellationToken)"/>.
    /// - <see cref="Tag.ITagApi.GetTopAlbumsAsync(string, int?, int?, CancellationToken)"/>.
    /// - <see cref="User.IUserApi.GetTopAlbumsAsync(string, User.TimePeriod?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public Uri? Url { get; private set; }

    /// <summary>
    /// Mbid of this album.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// </remarks>
    public string? Mbid { get; private set; }

    /// <summary>
    /// Number of people who listened to this album.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="IAlbumApi.GetInfoByNameAsync(string, string, string?, bool, string?, CancellationToken)"/>.
    /// - <see cref="IAlbumApi.GetInfoByMbidAsync(string, string?, bool, string?, CancellationToken)"/>.
    /// </remarks>
    public int? Listeners { get; private set; }

    /// <summary>
    /// Total amount of plays this album has.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="IAlbumApi.GetInfoByNameAsync(string, string, string?, bool, string?, CancellationToken)"/>.
    /// - <see cref="IAlbumApi.GetInfoByMbidAsync(string, string?, bool, string?, CancellationToken)"/>.
    /// - <see cref="IArtistApi.GetTopAlbumsByNameAsync(string, bool, int?, int?, CancellationToken)"/>
    /// - <see cref="IArtistApi.GetTopAlbumsByMbidAsync(string, int?, int?, CancellationToken)"/>
    /// </remarks>
    public int? PlayCount { get; internal set; }

    /// <summary>
    /// Amount of plays of this album the user has for which the request has been made.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="IAlbumApi.GetInfoByNameAsync(string, string, string?, bool, string?, CancellationToken)"/> with provided username.
    /// - <see cref="IAlbumApi.GetInfoByMbidAsync(string, string?, bool, string?, CancellationToken)"/> with provided username.
    /// - <see cref="User.IUserApi.GetTopAlbumsAsync(string, User.TimePeriod?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public int? UserPlayCount { get; internal set; }

    /// <summary>
    /// If this album can be streamed (previewed or full) on last.fm.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="IAlbumApi.SearchAsync(string, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public bool? IsStreamable { get; private set; }

    /// <summary>
    /// Images associated with this album.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// </remarks>
    public Dictionary<ImageSize, Uri> Images { get; private set; } = [];

    /// <summary>
    /// Info about the artist of this album.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="IAlbumApi.GetInfoByNameAsync(string, string, string?, bool, string?, CancellationToken)"/>.
    /// - <see cref="IAlbumApi.GetInfoByMbidAsync(string, string?, bool, string?, CancellationToken)"/>.
    /// - <see cref="Tag.ITagApi.GetTopAlbumsAsync(string, int?, int?, CancellationToken)"/>.
    /// - <see cref="User.IUserApi.GetTopAlbumsAsync(string, User.TimePeriod?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public ArtistInfo? Artist { get; private set; }

    /// <summary>
    /// The track list of this album.
    /// </summary>
    /// <remarks>
    /// May be empty.
    /// Guaranteed to be available when using:
    /// - <see cref="IAlbumApi.GetInfoByNameAsync(string, string, string?, bool, string?, CancellationToken)"/>.
    /// - <see cref="IAlbumApi.GetInfoByMbidAsync(string, string?, bool, string?, CancellationToken)"/>.
    /// </remarks>
    public IReadOnlyList<TrackInfo> Tracks { get; private set; } = [];

    /// <summary>
    /// List of most used tags for this album.
    /// </summary>
    /// <remarks>
    /// May be emptry.
    /// Guaranteed to be available when using:
    /// - <see cref="IAlbumApi.GetInfoByNameAsync(string, string, string?, bool, string?, CancellationToken)"/>.
    /// - <see cref="IAlbumApi.GetInfoByMbidAsync(string, string?, bool, string?, CancellationToken)"/>.
    /// </remarks>
    public IReadOnlyList<TagInfo> TopTags { get; private set; } = [];

    /// <summary>
    /// The wiki of this album
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="IAlbumApi.GetInfoByNameAsync(string, string, string?, bool, string?, CancellationToken)"/>.
    /// - <see cref="IAlbumApi.GetInfoByMbidAsync(string, string?, bool, string?, CancellationToken)"/>.
    /// </remarks>
    public WikiInfo? Wiki { get; private set; }

    internal static AlbumInfo FromJson(JsonElement root)
    {
      var album = root.TryGetProperty("album", out var albumProp) ? albumProp : root;

      // name might either be in the name or #text property
      var name = album.TryGetProperty("title", out var nameProp)
          ? nameProp.GetString() ?? ""
          : album.TryGetProperty("name", out var altNameProp)
              ? altNameProp.GetString() ?? ""
              : album.TryGetProperty("#text", out var textProp)
                  ? textProp.GetString() ?? ""
                  : "";

      // throw if name is empty
      if (string.IsNullOrEmpty(name))
        throw new JsonException("Album json malformed - name could not be parsed");

      int? playCount = null;
      if (album.TryGetProperty("playcount", out var playCountProp) && JsonHelper.TryParseNumber<int>(playCountProp, out var playCountNum))
        playCount = playCountNum;

      int? userPlayCount = null;
      if (album.TryGetProperty("userplaycount", out var userPlayCountProp) && JsonHelper.TryParseNumber<int>(userPlayCountProp, out var userPlayCountNum))
        userPlayCount = userPlayCountNum;

      int? listeners = null;
      if (album.TryGetProperty("listeners", out var listenerProp) && JsonHelper.TryParseNumber<int>(listenerProp, out var listenersNum))
        listeners = listenersNum;

      var isStreamable = album.TryGetProperty("streamable", out var streamableProp)
              ? streamableProp.GetString() == "1"
              : (bool?)null;

      ArtistInfo? artist = album.TryGetProperty("artist", out var artistProp) ? ArtistInfo.FromJson(artistProp) : null;

      IReadOnlyList<TrackInfo> tracks = [];
      if (album.TryGetProperty("tracks", out var topTracksElement) && topTracksElement.TryGetProperty("track", out var trackArray))
        tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

      IReadOnlyList<TagInfo> tags = [];
      if (album.TryGetProperty("tags", out var tagsProp) && tagsProp.TryGetProperty("tag", out var tagArray))
        tags = JsonHelper.MakeListFromJsonArray(tagArray, TagInfo.FromJson);

      WikiInfo? wiki = null;
      if (album.TryGetProperty("wiki", out var wikiProp))
        wiki = WikiInfo.FromJson(wikiProp);

      return new AlbumInfo
      {
        Name = name,
        Url = album.TryGetProperty("url", out var urlProperty) ? new Uri(urlProperty.GetString() ?? "") : (artist != null ? UriHelper.MakeAlbumUri(artist.Name, name) : null),
        Mbid = root.TryGetProperty("mbid", out var mbidProp) ? mbidProp.GetString() : null,
        Images = JsonHelper.ParseImageArray(root),
        Listeners = listeners,
        UserPlayCount = userPlayCount ?? playCount,
        PlayCount = playCount,
        IsStreamable = isStreamable,
        Artist = artist,
        Tracks = tracks,
        TopTags = tags,
        Wiki = wiki
      };
    }
  }
}
