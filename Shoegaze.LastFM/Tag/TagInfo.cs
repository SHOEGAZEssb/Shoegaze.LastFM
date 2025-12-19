using System.Text.Json;

namespace Shoegaze.LastFM.Tag
{
  /// <summary>
  /// A last.fm tag.
  /// </summary>
  public class TagInfo : IJsonDeserializable<TagInfo>
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
    /// Amount of times the track this request was sent for
    /// has been tagged with this tag.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="Track.ITrackApi.GetTopTagsByNameAsync(string, string, bool, CancellationToken)"/>.
    /// </remarks>
    public int? CountOnTrack { get; set; }

    /// <summary>
    /// A weighted count of how often the tag was applied
    /// to the album this request has been sent for, with a maximum of 100.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="Album.IAlbumApi.GetTopTagsByNameAsync(string, string, bool, CancellationToken)"/>.
    /// </remarks>
    public int? WeightOnAlbum { get; set; }

    /// <summary>
    /// Amount of users that have used this tag.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="ITagApi.GetInfoAsync(string, CancellationToken)"/>.
    /// - <see cref="Chart.IChartApi.GetTopTagsAsync(int?, int?, CancellationToken)"/>.
    /// </remarks>
    public int? Reach { get; set; }

    /// <summary>
    /// Total number of times this tag has been used.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="ITagApi.GetInfoAsync(string, CancellationToken)"/>.
    /// - <see cref="Chart.IChartApi.GetTopTagsAsync(int?, int?, CancellationToken)"/>.
    /// </remarks>
    public int? Taggings { get; set; }

    /// <summary>
    /// Amount of times this tag has been used by the user.
    /// </summary>
    /// <remarks>
    /// This information may be null.
    /// Guaranteed to be included when using:
    /// - <see cref="User.IUserApi.GetTopTracksAsync(string?, User.TimePeriod?, int?, int?, CancellationToken)"/> with included username.
    /// </remarks>
    public int? UserUsedCount { get; set; }

    /// <summary>
    /// The wiki of this tag.
    /// </summary>
    /// <remarks>
    /// May be null.
    /// Guaranteed to be available when using:
    /// - <see cref="ITagApi.GetInfoAsync(string, CancellationToken)"/>.
    /// </remarks>
    public WikiInfo? Wiki { get; set; }

    internal static TagInfo FromJson(JsonElement root)
    {
      var name = root.GetProperty("name").GetString()!;

      int? count = null;
      if (root.TryGetProperty("count", out var countElement) && JsonHelper.TryParseNumber<int>(countElement, out var countValue))
        count = countValue;

      int? reach = null;
      if (root.TryGetProperty("reach", out var reachElement) && JsonHelper.TryParseNumber<int>(reachElement, out var reachValue))
        reach = reachValue;

      int? total = null;
      if (root.TryGetProperty("total", out var totalElement) && JsonHelper.TryParseNumber<int>(totalElement, out var totalValue))
        total = totalValue;

      int? taggings = null;
      if (root.TryGetProperty("taggings", out var taggingsProp) && JsonHelper.TryParseNumber<int>(taggingsProp, out var taggingsValue))
        taggings = taggingsValue;

      WikiInfo? wiki = null;
      if (root.TryGetProperty("wiki", out var wikiProp) && wikiProp.EnumerateObject().Any())
        wiki = WikiInfo.FromJson(wikiProp);

      return new TagInfo
      {
        Name = name,
        Url = root.TryGetProperty("url", out var urlProp) ? new Uri(urlProp.GetString()!) : UriHelper.MakeTagUri(name),
        CountOnTrack = count,
        WeightOnAlbum = count,
        UserUsedCount = count,
        Reach = reach,
        Taggings = taggings ?? total ?? count,
        Wiki = wiki
      };
    }
  }
}