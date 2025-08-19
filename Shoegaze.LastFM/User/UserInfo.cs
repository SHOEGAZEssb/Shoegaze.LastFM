using System.Text.Json;

namespace Shoegaze.LastFM.User;

/// <summary>
/// A Last.fm user's profile and statistics.
/// </summary>
public sealed class UserInfo : IJsonDeserializable<UserInfo>
{
  /// <summary>
  /// The name of the user.
  /// </summary>
  public required string Username { get; set; }

  /// <summary>
  /// The real name of the user.
  /// </summary>
  /// <remarks>
  /// May be null or empty.
  /// </remarks>
  public string? RealName { get; private set; }

  /// <summary>
  /// The last.fm url of the user.
  /// </summary>
  public required Uri Url { get; set; }

  /// <summary>
  /// The country of the user.
  /// </summary>
  /// <remarks>
  /// May be null or empty.
  /// </remarks>
  public string? Country { get; private set; }

  /// <summary>
  /// The age of the user.
  /// </summary>
  /// <remarks>
  /// May be null or 0.
  /// </remarks>
  public int? Age { get; private set; }

  /// <summary>
  /// The gender of the user.
  /// </summary>
  /// <remarks>
  /// May be null.
  /// </remarks>
  public string? Gender { get; private set; }

  /// <summary>
  /// Indicates if the user is a last.fm pro user.
  /// </summary>
  /// <remarks>
  /// May be null.
  /// </remarks>
  public bool? IsSubscriber { get; private set; }

  /// <summary>
  /// The total amount of plays the user has.
  /// </summary>
  /// <remarks>
  /// May be null.
  /// </remarks>
  public int? PlayCount { get; private set; }

  /// <summary>
  /// Number of artists this user has listened to.
  /// </summary>
  /// <remarks>
  /// May be null.
  /// Guaranteed to be available when using:
  /// - <see cref="IUserApi.GetInfoAsync(string?, CancellationToken)"/>
  /// </remarks>
  public int? ArtistCount { get; private set; }

  /// <summary>
  /// Number of individual tracks this user has listened to.
  /// </summary>
  /// <remarks>
  /// May be null.
  /// Guaranteed to be available when using:
  /// - <see cref="IUserApi.GetInfoAsync(string?, CancellationToken)"/>
  /// </remarks>
  public int? TrackCount { get; private set; }

  /// <summary>
  /// Number of albums this user has listened to.
  /// </summary>
  /// <remarks>
  /// May be null.
  /// Guaranteed to be available when using:
  /// - <see cref="IUserApi.GetInfoAsync(string?, CancellationToken)"/>
  /// </remarks>
  public int? AlbumCount { get; private set; }

  /// <summary>
  /// Amount of playlists this user has.
  /// </summary>
  /// <remarks>
  /// May be null or 0.
  /// </remarks>
  public int? Playlists { get; private set; }

  /// <summary>
  /// When this user registered on last.fm.
  /// </summary>
  /// <remarks>
  /// May be null.
  /// </remarks>
  public DateTime? RegisteredDate { get; private set; }

  /// <summary>
  /// Images of this user.
  /// </summary>
  /// <remarks>
  /// May be empty.
  /// </remarks>
  public IReadOnlyDictionary<ImageSize, Uri> Images { get; private set; } = new Dictionary<ImageSize, Uri>();

  /// <summary>
  /// Gets the largest available image of this user.
  /// </summary>
  public Uri? ImageUrl =>
      Images.TryGetValue(ImageSize.ExtraLarge, out var xl) ? xl :
      Images.TryGetValue(ImageSize.Large, out var l) ? l :
      Images.TryGetValue(ImageSize.Medium, out var m) ? m :
      Images.TryGetValue(ImageSize.Small, out var s) ? s :
      Images.TryGetValue(ImageSize.Unknown, out var u) ? u : null;

  /// <summary>
  /// Parses a <c>JsonElement</c> representing a Last.fm user into a <see cref="UserInfo"/> instance.
  /// </summary>
  internal static UserInfo FromJson(JsonElement root)
  {
    var images = JsonHelper.ParseImageArray(root);

    var username = root.GetProperty("name").GetString()!;
    return new UserInfo
    {
      Username = username,
      RealName = root.TryGetProperty("realname", out var realNameProp) ? realNameProp.GetString() : null,
      Url = UriHelper.MakeUserUri(username)!,
      Country = root.GetProperty("country").GetString()!,
      Age = root.TryGetProperty("age", out var ageProp) && int.TryParse(ageProp.GetString(), out var age) ? age : null,
      Gender = root.TryGetProperty("gender", out var genderProp) ? genderProp.GetString() : null,
      IsSubscriber = root.GetProperty("subscriber").GetString() == "1",
      PlayCount = int.Parse(root.GetProperty("playcount").GetString()!, System.Globalization.CultureInfo.InvariantCulture),
      ArtistCount = root.TryGetProperty("artist_count", out var artistCountProp) ? int.Parse(artistCountProp.GetString()!, System.Globalization.CultureInfo.InvariantCulture) : null,
      TrackCount = root.TryGetProperty("track_count", out var trackCountProp) ? int.Parse(trackCountProp.GetString()!, System.Globalization.CultureInfo.InvariantCulture) : null,
      AlbumCount = root.TryGetProperty("album_count", out var albumCountProp) ? int.Parse(albumCountProp.GetString()!, System.Globalization.CultureInfo.InvariantCulture) : null,
      Playlists = int.Parse(root.GetProperty("playlists").GetString()!, System.Globalization.CultureInfo.InvariantCulture),
      RegisteredDate = DateTimeOffset
        .FromUnixTimeSeconds(long.Parse(root.GetProperty("registered").GetProperty("unixtime").GetString()!, System.Globalization.CultureInfo.InvariantCulture))
        .DateTime,
      Images = images
    };
  }
}
