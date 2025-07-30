using System.Text.Json;

namespace Shoegaze.LastFM.User;

/// <summary>
/// Represents a Last.fm user's profile and statistics.
/// </summary>
public class UserInfo
{
  public string Username { get; set; } = default!;
  public string RealName { get; set; } = default!;
  public string Url { get; set; } = default!;
  public string Country { get; set; } = default!;
  public int? Age { get; set; }
  public string? Gender { get; set; } = default!;
  public bool IsSubscriber { get; set; }
  public int Playcount { get; set; }

  /// <summary>
  /// Number of artists this user has listened to.
  /// </summary>
  /// <remarks>
  /// May be null.
  /// Guaranteed to be available when using:
  /// - <see cref="IUserApi.GetInfoAsync(string?, CancellationToken)"/>
  /// </remarks>
  public int? ArtistCount { get; set; }

  /// <summary>
  /// Number of individual tracks this user has listened to.
  /// </summary>
  /// <remarks>
  /// May be null.
  /// Guaranteed to be available when using:
  /// - <see cref="IUserApi.GetInfoAsync(string?, CancellationToken)"/>
  /// </remarks>
  public int? TrackCount { get; set; }

  /// <summary>
  /// Number of albums this user has listened to.
  /// </summary>
  /// <remarks>
  /// May be null.
  /// Guaranteed to be available when using:
  /// - <see cref="IUserApi.GetInfoAsync(string?, CancellationToken)"/>
  /// </remarks>
  public int? AlbumCount { get; set; }
  public int Playlists { get; set; }
  public DateTime RegisteredDate { get; set; }
  public IReadOnlyDictionary<ImageSize, Uri> Images { get; set; } = new Dictionary<ImageSize, Uri>();

  public Uri? ImageUrl =>
      Images.TryGetValue(ImageSize.Mega, out var mega) ? mega :
      Images.TryGetValue(ImageSize.ExtraLarge, out var xl) ? xl :
      Images.TryGetValue(ImageSize.Large, out var l) ? l :
      Images.TryGetValue(ImageSize.Medium, out var m) ? m :
      Images.TryGetValue(ImageSize.Small, out var s) ? s :
      Images.TryGetValue(ImageSize.Unknown, out var u) ? u : null;


  /// <summary>
  /// Parses a <c>JsonElement</c> representing a Last.fm user into a <see cref="UserInfo"/> instance.
  /// </summary>
  public static UserInfo FromJson(JsonElement root)
  {
    var images = JsonHelper.ParseImageArray(root);

    return new UserInfo
    {
      Username = root.GetProperty("name").GetString()!,
      RealName = root.GetProperty("realname").GetString()!,
      Url = root.GetProperty("url").GetString()!,
      Country = root.GetProperty("country").GetString()!,
      Age = root.TryGetProperty("age", out var ageProp) && int.TryParse(ageProp.GetString(), out var age) ? age : null,
      Gender = root.TryGetProperty("gender", out var genderProp) ? genderProp.GetString() : null,
      IsSubscriber = root.GetProperty("subscriber").GetString() == "1",
      Playcount = int.Parse(root.GetProperty("playcount").GetString()!),
      ArtistCount = root.TryGetProperty("artist_count", out var artistCountProp) ? int.Parse(artistCountProp.GetString()!) : null,
      TrackCount = root.TryGetProperty("track_count", out var trackCountProp) ? int.Parse(trackCountProp.GetString()!) : null,
      AlbumCount = root.TryGetProperty("album_count", out var albumCountProp) ? int.Parse(albumCountProp.GetString()!) : null,
      Playlists = int.Parse(root.GetProperty("playlists").GetString()!),
      RegisteredDate = DateTimeOffset
        .FromUnixTimeSeconds(long.Parse(root.GetProperty("registered").GetProperty("unixtime").GetString()!))
        .DateTime,
      Images = images
    };
  }


}
