using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.User
{
  public sealed class TopTag
  {
    public required string Name { get; init; }
    public required int Count { get; init; }
    public required string Url { get; init; }

    internal static TopTag FromJson(JsonElement json)
    {
      return new TopTag
      {
        Name = json.GetProperty("name").GetString()!,
        Count = int.Parse(json.GetProperty("count").GetString()!),
        Url = json.GetProperty("url").GetString()!
      };
    }
  }

}
