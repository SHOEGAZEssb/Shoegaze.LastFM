using Shoegaze.LastFM.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.User
{
  public sealed class UserTopTag : TagBase
  {
    public required int Count { get; init; }

    internal new static UserTopTag FromJson(JsonElement json)
    {
      var tagBase = TagBase.FromJson(json);
      return new UserTopTag
      {
        Name = tagBase.Name,
        Url = tagBase.Url,
        Count = int.Parse(json.GetProperty("count").GetString()!)
      };
    }
  }

}
