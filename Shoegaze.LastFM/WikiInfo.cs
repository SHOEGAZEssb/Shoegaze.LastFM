using System.Text.Json;

namespace Shoegaze.LastFM
{
  public sealed class WikiInfo
  {
    public DateTime? Published { get; set; }
    public string Summary { get; set; } = "";
    public string Content { get; set; } = "";

    internal static WikiInfo FromJson(JsonElement root)
    {
      DateTime? published = null;
      if (root.TryGetProperty("published", out var pubProp))
      {
        var raw = pubProp.GetString();
        if (DateTime.TryParse(raw, out var parsed))
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
