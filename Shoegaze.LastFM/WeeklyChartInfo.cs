using System.Text.Json;

namespace Shoegaze.LastFM
{
  public class WeeklyChartInfo : IJsonDeserializable<WeeklyChartInfo>
  {
    #region Properties

    public DateTime From { get; private set; }

    public DateTime To { get; private set; }

    #endregion Propertis

    internal static WeeklyChartInfo FromJson(JsonElement root)
    {
      return new WeeklyChartInfo
      {
        From = DateTimeOffset.FromUnixTimeSeconds(long.Parse(root.GetProperty("from").GetString()!, System.Globalization.CultureInfo.InvariantCulture)).DateTime,
        To = DateTimeOffset.FromUnixTimeSeconds(long.Parse(root.GetProperty("to").GetString()!, System.Globalization.CultureInfo.InvariantCulture)).DateTime,
      };
    }
  }
}
