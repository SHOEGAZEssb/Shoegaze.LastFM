using System.Text.Json;

namespace Shoegaze.LastFM.Tag
{
  public class TagInfo
  {
    /// <summary>
    /// Name of the tag.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Link to the last.fm page of this tag.
    /// </summary>
    public required Uri Url { get; init; }

    /// <summary>
    /// Amount of times this tag has been used by the user.
    /// </summary>
    /// <remarks>
    /// This information may be absent.
    /// Guaranteed to be included when using:
    /// - <see cref="User.IUserApi.GetTopTracksAsync(string?, User.TimePeriod?, int?, int?, CancellationToken)"/> with included username.
    /// </remarks>
    public int? UserUsedCount { get; init; }

    internal static TagInfo FromJson(JsonElement root)
    {
      int? count = root.TryGetProperty("count", out var countElement) ? (int.TryParse(countElement.GetString()!, out int result) ? result : null) : null;
      return new TagInfo
      {
        Name = root.GetProperty("name").GetString()!,
        Url = new Uri(root.GetProperty("url").GetString()!),
        UserUsedCount = count
      };
    }
  }
}
