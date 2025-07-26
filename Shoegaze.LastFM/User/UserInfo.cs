namespace Shoegaze.LastFM.User;

/// <summary>
/// Represents a Last.fm user's profile and statistics.
/// </summary>
public class UserInfo
{
  public string Id { get; init; } = default!;
  public string Username { get; init; } = default!;
  public string RealName { get; init; } = default!;
  public string Url { get; init; } = default!;
  public string Country { get; init; } = default!;
  public int Age { get; init; }
  public string Gender { get; init; } = default!;
  public bool IsSubscriber { get; init; }
  public int Playcount { get; init; }
  public int Playlists { get; init; }
  public DateTime RegisteredDate { get; init; }
  public string? ImageUrl { get; init; }
}
