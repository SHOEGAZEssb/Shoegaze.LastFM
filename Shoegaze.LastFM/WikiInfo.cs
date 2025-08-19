using System.Globalization;
using System.Text.Json;

namespace Shoegaze.LastFM
{
  /// <summary>
  /// The wiki of a last.fm object.
  /// </summary>
  public sealed class WikiInfo : IJsonDeserializable<WikiInfo>
  {
    /// <summary>
    /// When the wiki was published.
    /// </summary>
    public DateTime? Published { get; private set; }

    /// <summary>
    /// Shorter summary of the wiki.
    /// </summary>
    public string Summary { get; private set; } = "";

    /// <summary>
    /// The wiki content.
    /// </summary>
    public string Content { get; private set; } = "";

    internal static WikiInfo FromJson(JsonElement root)
    {
      DateTime? published = null;
      if (root.TryGetProperty("published", out var pubProp))
      {
        var raw = pubProp.GetString();
        if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, out var parsed))
          published = parsed;
      }

      var summary = root.TryGetProperty("summary", out var sumProp) ? sumProp.GetString() ?? "" : "";
      var content = root.TryGetProperty("content", out var contProp) ? contProp.GetString() ?? "" : "";

      return new WikiInfo
      {
        Published = published,
        Summary = summary,
        Content = content
      };
    }
  }
}