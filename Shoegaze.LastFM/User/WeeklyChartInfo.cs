using System.Text.Json;

namespace Shoegaze.LastFM.User
{
  public class WeeklyChartInfo : IJsonDeserializable<WeeklyChartInfo>
  {
    #region Properties

    public DateTime From { get; set; }

    public DateTime To { get; set; }

    #endregion Propertis

    internal static WeeklyChartInfo FromJson(JsonElement root)
    {
      return new WeeklyChartInfo
      {
        From = DateTimeOffset.FromUnixTimeSeconds(long.Parse(root.GetProperty("from").GetString()!)).DateTime,
        To = DateTimeOffset.FromUnixTimeSeconds(long.Parse(root.GetProperty("to").GetString()!)).DateTime,
      };
    }
  }
}
