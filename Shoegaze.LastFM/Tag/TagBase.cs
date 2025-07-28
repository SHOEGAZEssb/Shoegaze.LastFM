using Shoegaze.LastFM.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Tag
{
  public class TagBase
  {
    public required string Name { get; init; }
    public required Uri Url { get; init; }

    internal static TagBase FromJson(JsonElement root)
    {
      return new TagBase
      {
        Name = root.GetProperty("name").GetString()!,
        Url = new Uri(root.GetProperty("url").GetString()!)
      };
    }
  }
}
