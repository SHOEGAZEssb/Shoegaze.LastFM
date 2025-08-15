using System.Text.Json;

namespace Shoegaze.LastFM
{
  internal interface IJsonDeserializable<out T>
  {
    internal static T FromJson(JsonElement root) => throw new NotImplementedException();
  }
}
