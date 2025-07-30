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
  }

  internal static class UriHelper
  {
    private const string BASEURL = "https://www.last.fm/";
    private const string MUSICBASEURL = "https://www.last.fm/music/";

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
