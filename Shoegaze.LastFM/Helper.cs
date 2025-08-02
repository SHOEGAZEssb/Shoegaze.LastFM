using Shoegaze.LastFM.User;
using System.Text.Json;

namespace Shoegaze.LastFM
{
  internal static class JsonHelper
  {
    public static Dictionary<ImageSize, Uri> ParseImageArray(JsonElement root)
    {
      var dictionary = new Dictionary<ImageSize, Uri>();
      if (root.TryGetProperty("image", out var imageToken))
      {
        switch (imageToken.ValueKind)
        {
          case JsonValueKind.Array:
            foreach (var img in imageToken.EnumerateArray())
              ParseImageArray(img, dictionary);
            break;

          case JsonValueKind.Object:
            ParseImageArray(imageToken, dictionary);
            break;

          case JsonValueKind.String:
            var url = imageToken.GetString();
            if (!string.IsNullOrWhiteSpace(url))
              dictionary[ImageSize.Unknown] = new Uri(url);
            break;
        }
      }

      return dictionary;
    }

    public static void ParseImageArray(JsonElement imageElement, Dictionary<ImageSize, Uri> target)
    {
      if (imageElement.ValueKind != JsonValueKind.Object)
        return;

      if (!imageElement.TryGetProperty("#text", out var urlProp))
        return;

      var size = ImageSize.Unknown;
      if (imageElement.TryGetProperty("size", out var sizeProp))
      {
        var sizeStr = sizeProp.GetString() ?? "";
        size = sizeStr.ToLowerInvariant() switch
        {
          "small" => ImageSize.Small,
          "medium" => ImageSize.Medium,
          "large" => ImageSize.Large,
          "extralarge" => ImageSize.ExtraLarge,
          _ => ImageSize.Unknown
        };
      }

      var url = urlProp.GetString();
      if (!string.IsNullOrWhiteSpace(url) && !target.ContainsKey(size))
        target[size] = new Uri(url);
    }

    public static bool TryParseNumber<T>(JsonElement element, out T value) where T : struct
    {
      if (element.ValueKind == JsonValueKind.String)
      {
        var str = element.GetString();
        if (typeof(T) == typeof(int) && int.TryParse(str, out var i))
        {
          value = (T)(object)i;
          return true;
        }
        else if (typeof(T) == typeof(long) && long.TryParse(str, out var l))
        {
          value = (T)(object)l;
          return true;
        }
        else if (typeof(T) == typeof(double) && double.TryParse(str, out var d))
        {
          value = (T)(object)d;
          return true;
        }
      }
      else if (element.ValueKind == JsonValueKind.Number)
      {
        if (typeof(T) == typeof(int) && element.TryGetInt32(out var i))
        {
          value = (T)(object)i;
          return true;
        }
        else if (typeof(T) == typeof(long) && element.TryGetInt64(out var l))
        {
          value = (T)(object)l;
          return true;
        }
        else if (typeof(T) == typeof(double) && element.TryGetDouble(out var d))
        {
          value = (T)(object)d;
          return true;
        }
      }

      value = default;
      return false;
    }

    public static T ParseNumber<T>(JsonElement element) where T : struct
    {
      if (element.ValueKind == JsonValueKind.String)
      {
        var str = element.GetString();
        if (typeof(T) == typeof(int))
          return (T)(object)int.Parse(str!);
        if (typeof(T) == typeof(long))
          return (T)(object)long.Parse(str!);
        if (typeof(T) == typeof(double))
          return (T)(object)double.Parse(str!);
      }
      else if (element.ValueKind == JsonValueKind.Number)
      {
        if (typeof(T) == typeof(int))
          return (T)(object)element.GetInt32();
        if (typeof(T) == typeof(long))
          return (T)(object)element.GetInt64();
        if (typeof(T) == typeof(double))
          return (T)(object)element.GetDouble();
      }

      throw new FormatException($"Cannot parse JSON element as {typeof(T).Name}: {element}");
    }
  }

  internal static class UriHelper
  {
    private const string MUSICBASEURL = "https://www.last.fm/music/";
    private const string USERBASEURL = "https://www.last.fm/user/";
    private const string TAGBASEURL = "https://www.last.fm/tag/";

    public static Uri? MakeArtistUri(string artist)
    {
      if (string.IsNullOrEmpty(artist))
        return null;

      return new Uri(MUSICBASEURL + artist);
    }

    public static Uri? MakeAlbumUri(string artist, string album)
    {
      if (string.IsNullOrEmpty(artist) || string.IsNullOrEmpty(album))
        return null;

      return new Uri($"{MUSICBASEURL}{artist}/{album}");
    }

    public static Uri? MakeUserUri(string username)
    {
      if (string.IsNullOrEmpty(username))
        return null;

      return new Uri($"{USERBASEURL}{username}");
    }

    public static Uri MakeTagUri(string tag)
    {
      return new Uri($"{TAGBASEURL}{tag.Replace(" ", "+")}");
    }
  }

  internal static class TimePeriodExtensions
  {
    public static string ToApiString(this TimePeriod period) => period switch
    {
      TimePeriod.Overall => "overall",
      TimePeriod.SevenDay => "7day",
      TimePeriod.OneMonth => "1month",
      TimePeriod.ThreeMonth => "3month",
      TimePeriod.SixMonth => "6month",
      TimePeriod.TwelveMonth => "12month",
      _ => "overall"
    };
  }
}