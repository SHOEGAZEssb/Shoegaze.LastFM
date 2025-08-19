using System.Diagnostics.CodeAnalysis;

namespace Shoegaze.LastFM.Track
{
  /// <summary>
  /// Contains data for scrobbling.
  /// </summary>
  public class ScrobbleData
  {
    #region Properties

    /// <summary>
    /// Name of the artist to scrobble.
    /// </summary>
    public required string ArtistName { get; set; }

    /// <summary>
    /// Name of the track to scrobble.
    /// </summary>
    public required string TrackName { get; set; }

    /// <summary>
    /// Timestamp to scrobble.
    /// </summary>
    public required DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Name of the album to scrobble.
    /// Optional.
    /// </summary>
    public string? AlbumName { get; set; }

    /// <summary>
    /// Name of the album artist to scrobble.
    /// Optional.
    /// </summary>
    public string? AlbumArtistName { get; set; }

    /// <summary>
    /// If the user chose this track by themself.
    /// Optional.
    /// </summary>
    public bool? ChosenByUser { get; set; }

    /// <summary>
    /// Track number of the track on the album its from.
    /// Optional.
    /// </summary>
    public int? TrackNumber { get; set; }

    /// <summary>
    /// Duration of the track.
    /// Optional.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Arbitrary context for this track.
    /// Optional.
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    /// MusicBrainz ID of this track.
    /// Optional.
    /// </summary>
    public string? Mbid { get; set; }

    #endregion Properties

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="artistName">Name of the artist to scrobble.</param>
    /// <param name="trackName">Name of the track to scrobble.</param>
    /// <param name="playedAt">Timestamp to scrobble.</param>
    /// <param name="albumName">Name of the album to scrobble.</param>
    /// <param name="albumArtistName">Name of the album artist to scrobble.</param>
    /// <param name="trackNumber">Track number of the track on the album its from.</param>
    /// <param name="mbid">MusicBrainz ID of this track.</param>
    /// <param name="duration">Duration of the track.</param>
    /// <param name="chosenByUser">If the user chose this track by themself.</param>
    /// <param name="context">Arbitrary context for this track.</param>
    [SetsRequiredMembers]
#pragma warning disable IDE0290 // Use primary constructor
    public ScrobbleData(
#pragma warning restore IDE0290 // Use primary constructor
      string artistName, string trackName, DateTimeOffset playedAt,
      string? albumName = null, string? albumArtistName = null,
      int? trackNumber = null, string? mbid = null, TimeSpan? duration = null,
      bool? chosenByUser = null, string? context = null)
    {
      ArtistName = artistName;
      TrackName = trackName;
      Timestamp = playedAt;
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