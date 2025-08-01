using System.Text.Json;

namespace Shoegaze.LastFM.Tag
{
  public class TagInfo
  {
    /// <summary>
    /// Name of the tag.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Link to the last.fm page of this tag.
    /// </summary>
    public required Uri Url { get; set; }

    /// <summary>
    /// Amount of times the track this request has been sent for
    /// has been tagged with this tag.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="Track.ITrackApi.GetTopTagsByName(string, string, bool, CancellationToken)"/>
    /// - <see cref="Track.ITrackApi.GetTopTagsByMbid(string, bool, CancellationToken)"/>
    /// </remarks>
    public int? Count { get; set; }

    /// <summary>
    /// Amount of times this tag has been used by the user.
    /// </summary>
    /// <remarks>
    /// This information may be null.
    /// Guaranteed to be included when using:
    /// - <see cref="User.IUserApi.GetTopTracksAsync(string?, User.TimePeriod?, int?, int?, CancellationToken)"/> with included username.
    /// </remarks>
    public int? UserUsedCount { get; set; }

    internal static TagInfo FromJson(JsonElement root)
    {
      int? count = null;
      if (root.TryGetProperty("count", out var countElement) && JsonHelper.TryParseNumber<int>(countElement, out var countValue))
        count = countValue;

      return new TagInfo
      {
        Name = root.GetProperty("name").GetString()!,
        Url = new Uri(root.GetProperty("url").GetString()!),
        Count = count,
        UserUsedCount = count
      };
    }
  }
}
