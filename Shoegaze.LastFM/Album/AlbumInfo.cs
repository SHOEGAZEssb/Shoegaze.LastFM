using Shoegaze.LastFM.Artist;
using System.Text.Json;

namespace Shoegaze.LastFM.Album
{
  public class AlbumInfo : IChartable, ITagable, IJsonDeserializable<AlbumInfo>
  {
    /// <summary>
    /// Info about the artist of this album.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="User.IUserApi.GetTopAlbumsAsync(string, User.TimePeriod?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public ArtistInfo? Artist { get; set; }

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
    /// - <see cref="User.IUserApi.GetTopAlbumsAsync(string, User.TimePeriod?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public required Uri? Url { get; set; }

    /// <summary>
    /// Mbid of this album.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="User.IUserApi.GetTopAlbumsAsync(string, User.TimePeriod?, int?, int?, CancellationToken)"/>.
    /// </remarks>
    public string? Mbid { get; set; }

    /// <summary>
    /// Amount of plays of this album the user has for which the request has been made.
    /// </summary>
    /// <remarks>
    /// May be absent.
    /// Guaranteed to be available when using:
    /// - <see cref="User.IUserApi.GetTopAlbumsAsync(string, User.TimePeriod?, int?, int?, CancellationToken)/>.
    /// </remarks>
    public int? UserPlayCount { get; set; }

    /// <summary>
    /// Indicates the rank of a album when getting the top tracks for a user.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="User.IUserApi.GetTopAlbumsAsync(string, User.TimePeriod?, int?, int?, CancellationToken)/>.
    /// </remarks>
    public int? Rank { get; set; }

    public Dictionary<ImageSize, Uri> Images { get; set; } = [];

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
        throw new Exception("Album json malformed");

      int? rank = null;
      if (album.TryGetProperty("@attr", out var attributeProp) && attributeProp.TryGetProperty("rank", out var rankProp) && int.TryParse(rankProp.GetString()!, out var rankNum))
        rank = rankNum;

      int? playCount = null;
      if (album.TryGetProperty("playcount", out var playCountProp) && int.TryParse(playCountProp.GetString()!, out var playCountNum))
        playCount = playCountNum;

      ArtistInfo? artist = album.TryGetProperty("artist", out var _) ? ArtistInfo.FromJson(root) : null;

      return new AlbumInfo
      {
        Artist = artist,
        Name = name,
        Url = album.TryGetProperty("url", out var urlProperty) ? new Uri(urlProperty.GetString() ?? "") : (artist != null ? UriHelper.MakeAlbumUri(artist.Name, name) : null),
        Mbid = root.TryGetProperty("mbid", out var mbidProp) ? mbidProp.GetString() : null,
        Images = JsonHelper.ParseImageArray(root),
        Rank = rank,
        UserPlayCount = rank == null ? null : playCount // if rank is null, playCount is the global playCount
      };
    }
  }
}
