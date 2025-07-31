using System.Text.Json;

namespace Shoegaze.LastFM
{
  internal interface IJsonDeserializable<T>
  {
    internal static T FromJson(JsonElement root) => throw new NotImplementedException();
  }
}
