using System;
using System.Diagnostics.CodeAnalysis;

namespace Shoegaze.LastFM.Track
{
  public class ScrobbleData
  {
    #region Properties

    public required string ArtistName { get; set; }

    public required string TrackName { get; set; }

    public required DateTimeOffset Timestamp { get; set; }

    public string? AlbumName { get; set; }

    public string? AlbumArtistName { get; set; }

    public bool? ChosenByUser { get; set; }

    public int? TrackNumber { get; set; }

    public TimeSpan? Duration { get; set; }

    public string? Context { get; set; }

    public string? Mbid { get; set; }

    #endregion Properties

    #region Constructor

    [SetsRequiredMembers]
#pragma warning disable IDE0290 // Use primary constructor
    public ScrobbleData(
#pragma warning restore IDE0290 // Use primary constructor
      string artistName, string trackName, DateTime playedAtUtc,
      string? albumName = null, string? albumArtistName = null,
      int? trackNumber = null, string? mbid = null, TimeSpan? duration = null,
      bool? chosenByUser = null, string? context = null)
    {
      ArtistName = artistName;
      TrackName = trackName;
      Timestamp = new DateTimeOffset(
        DateTime.SpecifyKind(playedAtUtc, DateTimeKind.Utc));

      AlbumName = albumName;
      AlbumArtistName = albumArtistName;
      TrackNumber = trackNumber;
      Mbid = mbid;
      Duration = duration;
      ChosenByUser = chosenByUser;
      Context = context;
    }

    #endregion Constructor
  }
}
