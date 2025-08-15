using System.Globalization;
using System.Text.Json;

namespace Shoegaze.LastFM.Track
{
  internal enum IgnoredCode
  {
    None = 0,
    ArtistIgnored,
    TrackIgnored,
    TimestampTooOld,
    TimestampTooNew,
    ScrobbleLimitExceeded
  }

  public class ScrobbleInfo : IJsonDeserializable<ScrobbleInfo>
  {
    #region Properties

    public required string TrackName { get; set; }

    public bool IsTrackNameCorrected { get; private set; }

    public required string ArtistName { get; set; }

    public bool IsArtistNameCorrected { get; private set; }

    public DateTime? Timestamp { get; private set; }

    public string? AlbumName { get; private set; }

    public bool? IsAlbumNameCorrected { get; private set; }

    public string? AlbumArtistName { get; private set; }

    public bool? IsAlbumArtistNameCorrected { get; private set; }

    /// <summary>
    /// Indicates if the scrobble was ignored by last.fm.
    /// Main reason for this is usually that the timestamp
    /// of the scrobble is too old (can't be older than 2 weeks),
    /// or too new.
    /// </summary>
    public bool IsIgnored => _ignoredStatusCode != IgnoredCode.None;

    /// <summary>
    /// Ignored code as reported by the api.
    /// Seems to be always return 1 if ignored.
    /// </summary>
    private IgnoredCode _ignoredStatusCode = IgnoredCode.None;

    #endregion Properties

    internal static ScrobbleInfo FromJson(JsonElement root)
    {
      var trackName = ReadText(root, "track") ?? throw new InvalidDataException("Track name could not be parsed");
      var artistName = ReadText(root, "artist") ?? throw new InvalidDataException("Artist name could not be parsed");

      var albumName = NullIfEmpty(ReadText(root, "album"));
      var albumArtistName = NullIfEmpty(ReadText(root, "albumArtist"));

      var (ignoredCode, _) = ReadIgnored(root);

      var timestamp = NullIfEmpty(ReadText(root, "timestamp"));

      return new ScrobbleInfo
      {
        TrackName = trackName,
        IsTrackNameCorrected = ReadCorrected(root, "track"),
        ArtistName = artistName,
        IsArtistNameCorrected = ReadCorrected(root, "artist"),
        AlbumName = albumName,
        IsAlbumNameCorrected = albumName is null ? null : ReadCorrected(root, "album"),
        AlbumArtistName = albumArtistName,
        IsAlbumArtistNameCorrected = albumArtistName is null ? null : ReadCorrected(root, "albumArtist"),
        _ignoredStatusCode = ignoredCode,
        Timestamp = timestamp is null ? null : DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestamp, NumberStyles.Integer, CultureInfo.InvariantCulture)).UtcDateTime
      };
    }

    private static string? ReadText(JsonElement parent, string name)
    {
      if (!parent.TryGetProperty(name, out var el)) return null;

      if (el.ValueKind == JsonValueKind.Object)
      {
        if (el.TryGetProperty("#text", out var t))
          return t.GetString();
        return null;
      }

      return el.GetString();
    }

    private static bool ReadCorrected(JsonElement parent, string name)
    {
      if (!parent.TryGetProperty(name, out var el) || el.ValueKind != JsonValueKind.Object) return false;
      if (!el.TryGetProperty("corrected", out var c)) return false;
      var s = c.GetString();
      return s == "1" || s?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static (IgnoredCode code, string message) ReadIgnored(JsonElement parent)
    {
      if (!parent.TryGetProperty("ignoredMessage", out var el))
        return (IgnoredCode.None, "");

      string msg;
      if (el.ValueKind == JsonValueKind.Object)
      {
        if (el.TryGetProperty("#text", out var t)) msg = t.GetString() ?? "";
        else msg = el.GetString() ?? "";
      }
      else
      {
        msg = el.GetString() ?? "";
      }

      IgnoredCode code = IgnoredCode.None;
      if (el.ValueKind == JsonValueKind.Object && el.TryGetProperty("code", out var codeProp))
      {
        if (int.TryParse(codeProp.GetString(), out var n))
        {
          code = n switch
          {
            0 => IgnoredCode.None,
            1 => IgnoredCode.ArtistIgnored,
            2 => IgnoredCode.TrackIgnored,
            3 => IgnoredCode.TimestampTooOld,
            4 => IgnoredCode.TimestampTooNew,
            5 => IgnoredCode.ScrobbleLimitExceeded,
            _ => IgnoredCode.None
          };
        }
      }

      return (code, msg);
    }

    private static string? NullIfEmpty(string? value) =>
      string.IsNullOrWhiteSpace(value) ? null : value;
  }
}
