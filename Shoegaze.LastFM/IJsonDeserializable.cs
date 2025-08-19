using System.Text.Json;

namespace Shoegaze.LastFM
{
  /// <summary>
  /// Interface for an object that can be deserialized from
  /// a <see cref="JsonElement"/>.
  /// </summary>
  /// <typeparam name="T">Type of the deserialized object.</typeparam>
  internal interface IJsonDeserializable<out T>
  {
    internal static T FromJson(JsonElement root) => throw new NotImplementedException();
  }
}