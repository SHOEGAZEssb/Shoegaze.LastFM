using System.Text.Json;

namespace Shoegaze.LastFM.User;

/// <summary>
/// Represents a Last.fm user's profile and statistics.
/// </summary>
public class UserInfo
{
  public string? Id { get; init; } = default!;
  public string Username { get; init; } = default!;
  public string RealName { get; init; } = default!;
  public string Url { get; init; } = default!;
  public string Country { get; init; } = default!;
  public int? Age { get; init; }
  public string? Gender { get; init; } = default!;
  public bool IsSubscriber { get; init; }
  public int Playcount { get; init; }
  public int Playlists { get; init; }
  public DateTime RegisteredDate { get; init; }
  public IReadOnlyDictionary<ImageSize, string> Images { get; init; } = new Dictionary<ImageSize, string>();

  public string? ImageUrl =>
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
      Id = root.TryGetProperty("id", out var idProp) ? idProp.GetString() : null,
      Username = root.GetProperty("name").GetString()!,
      RealName = root.GetProperty("realname").GetString()!,
      Url = root.GetProperty("url").GetString()!,
      Country = root.GetProperty("country").GetString()!,
      Age = root.TryGetProperty("age", out var ageProp) && int.TryParse(ageProp.GetString(), out var age) ? age : null,
      Gender = root.TryGetProperty("gender", out var genderProp) ? genderProp.GetString() : null,
      IsSubscriber = root.GetProperty("subscriber").GetString() == "1",
      Playcount = int.Parse(root.GetProperty("playcount").GetString()!),
      Playlists = int.Parse(root.GetProperty("playlists").GetString()!),
      RegisteredDate = DateTimeOffset
        .FromUnixTimeSeconds(long.Parse(root.GetProperty("registered").GetProperty("unixtime").GetString()!))
        .DateTime,
      Images = images
    };
  }


}
