using System.Text.Json;

namespace Shoegaze.LastFM
{
  /// <summary>
  /// Info about a specific weekly chart of a last.fm user.
  /// </summary>
  public class WeeklyChartInfo : IJsonDeserializable<WeeklyChartInfo>
  {
    #region Properties

    /// <summary>
    /// Date start point.
    /// </summary>
    public DateTime From { get; private set; }

    /// <summary>
    /// Date end point.
    /// </summary>
    public DateTime To { get; private set; }

    #endregion Propertis

    internal static WeeklyChartInfo FromJson(JsonElement root)
    {
      return new WeeklyChartInfo
      {
        From = DateTimeOffset.FromUnixTimeSeconds(JsonHelper.ParseNumber<long>(root.GetProperty("from"))).DateTime,
        To = DateTimeOffset.FromUnixTimeSeconds(JsonHelper.ParseNumber<long>(root.GetProperty("to"))).DateTime
      };
    }
  }
}