using Shoegaze.LastFM.Tag;

namespace Shoegaze.LastFM.Track
{
  /// <summary>
  /// Access to track-related api endpoints.
  /// </summary>
  public class TrackApi : ITrackApi
  {
    private readonly ILastfmApiInvoker _invoker;

    internal TrackApi(ILastfmApiInvoker invoker) => _invoker = invoker;

    /// <summary>
    /// Get the metadata for a track.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist.</param>
    /// <param name="username">The username for the context of the request.
    /// If supplied, the user's playcount for this track and whether they have loved the track is included in the response.</param>
    /// <param name="autoCorrect">Transform misspelled artist and track names into correct artist and track names, returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the track metadata, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getInfo"/>.
    public async Task<ApiResult<TrackInfo>> GetInfoByNameAsync(string track, string artist, string? username = null, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "track", track },
        { "artist", artist }
      };

      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      return await GetInfoAsync(parameters, username, ct);
    }

    /// <summary>
    /// Get the metadata for a track.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the track.</param>
    /// <param name="username">The username for the context of the request.
    /// If supplied, the user's playcount for this track and whether they have loved the track is included in the response.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the track metadata, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getInfo"/>.
    public async Task<ApiResult<TrackInfo>> GetInfoByMbidAsync(string mbid, string? username = null, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "mbid", mbid }
      };

      return await GetInfoAsync(parameters, username, ct);
    }

    private async Task<ApiResult<TrackInfo>> GetInfoAsync(Dictionary<string, string> parameters, string? username, CancellationToken ct)
    {
      if (username != null)
        parameters.Add("username", username);

      var result = await _invoker.SendAsync("track.getInfo", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<TrackInfo>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var trackInfo = TrackInfo.FromJson(result.Data.RootElement.GetProperty("track"));
        if (username == null)
          trackInfo.UserPlayCount = null; // manual fix for duplicate playcount property possibility (if username is null, userplaycount will not be available)
        return ApiResult<TrackInfo>.Success(trackInfo);
      }
      catch (Exception ex)
      {
        return ApiResult<TrackInfo>.Failure(null, result.HttpStatus, "Failed to parse track info: " + ex.Message);
      }
    }

    /// <summary>
    /// Get the corrected name for a track.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the corrected track, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getCorrection"/>.
    public async Task<ApiResult<TrackInfo>> GetCorrectionAsync(string track, string artist, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "track", track },
        { "artist", artist }
      };

      var result = await _invoker.SendAsync("track.getCorrection", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<TrackInfo>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var trackInfo = TrackInfo.FromJson(result.Data.RootElement.GetProperty("corrections").GetProperty("correction"));
        return ApiResult<TrackInfo>.Success(trackInfo);
      }
      catch (Exception ex)
      {
        return ApiResult<TrackInfo>.Failure(null, result.HttpStatus, "Failed to parse track info: " + ex.Message);
      }
    }

    /// <summary>
    /// Get similar tracks to the given track.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist.</param>
    /// <param name="autoCorrect">Transform misspelled artist and track names into correct artist and track names, returning the correct version instead.</param>
    /// <param name="limit">Maximum number of similar tracks to return.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of similar tracks, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getSimilar"/>.
    public async Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarByNameAsync(string track, string artist, bool autoCorrect = true, int? limit = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, null);
      parameters.Add("track", track);
      parameters.Add("artist", artist);
      return await GetSimilarAsync(parameters, autoCorrect, ct);
    }

    /// <summary>
    /// Get similar tracks to the given track.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the track.</param>
    /// <param name="limit">Maximum number of similar tracks to return.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of similar tracks, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getSimilar"/>.
    public async Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarByMbidAsync(string mbid, int? limit = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, null);
      parameters.Add("mbid", mbid);
      return await GetSimilarAsync(parameters, true, ct);
    }

    private async Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarAsync(Dictionary<string, string> parameters, bool autoCorrect, CancellationToken ct)
    {
      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      var result = await _invoker.SendAsync("track.getSimilar", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<TrackInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var trackArray = result.Data.RootElement.GetProperty("similartracks").TryGetProperty("track", out var ta) ? ta : default;
        var tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

        return ApiResult<IReadOnlyList<TrackInfo>>.Success(tracks);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<TrackInfo>>.Failure(null, result.HttpStatus, "Failed to parse similar track list: " + ex.Message);
      }
    }

    /// <summary>
    /// Get the tags applied by an individual user to a track.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist.</param>
    /// <param name="username">Username to look up. If null, uses the authenticated session.</param>
    /// <param name="autoCorrect">Transform misspelled artist and track names into correct artist and track names, returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getTags"/>.
    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByNameAsync(string track, string artist, string? username = null, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "track", track },
        { "artist", artist }
      };

      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      return await GetUserTags(parameters, username, ct);
    }

    /// <summary>
    /// Get the tags applied by an individual user to a track.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the track.</param>
    /// <param name="username">Username to look up. If null, uses the authenticated session.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getTags"/>.
    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByMbidAsync(string mbid, string? username = null, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "mbid", mbid }
      };

      ParameterHelper.AddAutoCorrectParameter(parameters, true);

      return await GetUserTags(parameters, username, ct);
    }

    private async Task<ApiResult<IReadOnlyList<TagInfo>>> GetUserTags(Dictionary<string, string> parameters, string? username, CancellationToken ct)
    {
      if (username != null)
        parameters.Add("user", username);

      var result = await _invoker.SendAsync("track.getTags", parameters, username == null, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tagArray = result.Data.RootElement.GetProperty("tags").TryGetProperty("tag", out var ta) ? ta : default;
        var tags = JsonHelper.MakeListFromJsonArray(tagArray, TagInfo.FromJson);

        return ApiResult<IReadOnlyList<TagInfo>>.Success(tags);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(null, result.HttpStatus, "Failed to parse track tag list: " + ex.Message);
      }
    }

    /// <summary>
    /// Get the top tags for a track.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist.</param>
    /// <param name="autoCorrect">Transform misspelled artist and track names into correct artist and track names, returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getTopTags"/>.
    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByNameAsync(string track, string artist, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "track", track },
        { "artist", artist }
      };

      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);
      return await GetTopTags(parameters, ct);
    }

    /// <summary>
    /// Get the top tags for a track.
    /// </summary>
    /// <param name="mbid">MusicBrainz ID of the track.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.getTopTags"/>.
    [Obsolete("Currently broken on last.fms side, only returns an empty list.")]
    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByMbidAsync(string mbid, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "mbid", mbid }
      };

      return await GetTopTags(parameters, ct);
    }

    private async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTags(Dictionary<string, string> parameters, CancellationToken ct)
    {
      var result = await _invoker.SendAsync("track.getTopTags", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tagArray = result.Data.RootElement.GetProperty("toptags").TryGetProperty("tag", out var ta) ? ta : default;
        var tags = JsonHelper.MakeListFromJsonArray(tagArray, TagInfo.FromJson);

        foreach (var tag in tags)
        {
          tag.UserUsedCount = null; // not used in this function, but json property has same name as count
          tag.WeightOnAlbum = null;
        }

        return ApiResult<IReadOnlyList<TagInfo>>.Success(tags);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(null, result.HttpStatus, "Failed to parse track tag list: " + ex.Message);
      }
    }

    /// <summary>
    /// Search for a track by name.
    /// </summary>
    /// <param name="track">Name of the track.</param>
    /// <param name="artist">Name of the artist to narrow your search.</param>
    /// <param name="limit">Number of results per page (defaults to 30).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of found track sorted by relevance, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.search"/>.
    public async Task<ApiResult<PagedResult<TrackInfo>>> SearchAsync(string track, string? artist = null, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("track", track);

      if (artist != null)
        parameters.Add("artist", artist);

      var result = await _invoker.SendAsync("track.search", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<TrackInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var resultsProperty = result.Data.RootElement.GetProperty("results");
        var trackMatchesProperty = resultsProperty.GetProperty("trackmatches");
        var trackArray = trackMatchesProperty.TryGetProperty("track", out var ta) ? ta : default;
        var tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

        return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(resultsProperty, tracks));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<TrackInfo>>.Failure(null, result.HttpStatus, "Failed to parse tracks: " + ex.Message);
      }
    }

    /// <summary>
    /// Tag an album using a user supplied tag.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tag">Tag to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.addTags"/>.
    public async Task<ApiResult> AddTagsAsync(string trackName, string artistName, string tag, CancellationToken ct = default)
    {
      return await AddTagsAsync(trackName, artistName, [tag], ct);
    }

    /// <summary>
    /// Tag an album using a list of user supplied tag.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tags">Tags to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.addTags"/>.
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="tags"/> has more than 10 or less than 1 tag.</exception>
    public async Task<ApiResult> AddTagsAsync(string trackName, string artistName, IEnumerable<string> tags, CancellationToken ct = default)
    {
      var tagCount = tags.Count();
      if (tagCount > 10)
        throw new ArgumentOutOfRangeException(nameof(tags), "Only maximum of 10 tags allowed");
      if (tagCount == 0)
        throw new ArgumentOutOfRangeException(nameof(tags), "No tags supplied");

      var parameters = new Dictionary<string, string>
      {
        ["track"] = trackName,
        ["artist"] = artistName,
        ["tags"] = string.Join(",", tags)
      };

      var result = await _invoker.SendAsync("track.addTags", parameters, true, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      return ApiResult.Success();
    }

    /// <summary>
    /// Remove a users tag from an artist.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tag">Tag to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.removeTag"/>.
    public async Task<ApiResult> RemoveTagsAsync(string trackName, string artistName, string tag, CancellationToken ct = default)
    {
      return await RemoveTagsAsync(trackName, artistName, [tag], ct);
    }

    /// <summary>
    /// Remove a list of users tag from an artist.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tags">Tags to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.removeTag"/>.
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="tags"/> is empty.</exception>
    public async Task<ApiResult> RemoveTagsAsync(string trackName, string artistName, IEnumerable<string> tags, CancellationToken ct = default)
    {
      if (!tags.Any())
        throw new ArgumentOutOfRangeException(nameof(tags), "No tags to remove supplied");

      foreach (var tag in tags)
      {
        var parameters = new Dictionary<string, string>
        {
          ["track"] = trackName,
          ["artist"] = artistName,
          ["tags"] = tag
        };

        var result = await _invoker.SendAsync("track.removeTag", parameters, true, ct);
        if (!result.IsSuccess || result.Data == null)
          return ApiResult.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);
      }

      return ApiResult.Success();
    }

    /// <summary>
    /// Loves or unloves a track.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="loveState">The love state to set.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.love"/>.
    /// <seealso href="https://www.last.fm/api/show/track.unlove"/>.
    public async Task<ApiResult> SetLoveState(string trackName, string artistName, bool loveState, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["track"] = trackName,
        ["artist"] = artistName
      };

      var result = await _invoker.SendAsync(loveState ? "track.love" : "track.unlove", parameters, true, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      return ApiResult.Success();
    }

    /// <summary>
    /// Notify last.fm that a user has started listening to a track.
    /// This method requires authentication.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="albumArtistName">Name of the album artist.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.updateNowPlaying"/>.
    public async Task<ApiResult> UpdateNowPlayingAsync(string trackName, string artistName, string? albumName = null, string? albumArtistName = null, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["track"] = trackName,
        ["artist"] = artistName
      };

      if (albumName != null)
        parameters.Add("album", albumName);
      if (albumArtistName != null)
        parameters.Add("albumArtist", albumArtistName);

      var result = await _invoker.SendAsync("track.updateNowPlaying", parameters, true, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<ScrobbleInfo>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      return ApiResult.Success();
    }

    /// <summary>
    /// Add a single track-play to a users profile.
    /// This method requires authentication.
    /// </summary>
    /// <param name="scrobble">The scrobble data to scrobble.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a scrobble response, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.scrobble"/>.
    public async Task<ApiResult<ScrobbleInfo>> ScrobbleAsync(ScrobbleData scrobble, CancellationToken ct = default)
    {
      var response = await ScrobbleAsync([scrobble], ct);
      return new ApiResult<ScrobbleInfo>(response.Data?[0], response.LastFmStatus, response.HttpStatus, response.ErrorMessage );
    }

    /// <summary>
    /// Add a batch of track-play to a users profile.
    /// This method requires authentication.
    /// </summary>
    /// <param name="scrobbles">The scrobble data to scrobble (maximum of 50).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a scrobble response, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/track.scrobble"/>.
    /// <exception cref="ArgumentNullException">When <paramref name="scrobbles"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="scrobbles"/> has more than 50 or less than 1 scrobble.</exception>
    public async Task<ApiResult<IReadOnlyList<ScrobbleInfo>>> ScrobbleAsync(IEnumerable<ScrobbleData> scrobbles, CancellationToken ct = default)
    {
      ArgumentNullException.ThrowIfNull(scrobbles);
      var scrobbleArray = scrobbles.ToArray();
      if (scrobbleArray.Length == 0)
        throw new ArgumentOutOfRangeException(nameof(scrobbles), "No scrobbles provided");
      if (scrobbleArray.Length > 50)
        throw new ArgumentOutOfRangeException(nameof(scrobbles), "Maximum of 50 scrobbles allowed");

      var parameters = new Dictionary<string, string>();
      for (int i = 0; i < scrobbleArray.Length; i++)
      {
        if (scrobbleArray[i] == null)
          throw new ArgumentNullException(nameof(scrobbles), $"Scrobble at index {i} is null");

        // mandatory params
        parameters.Add($"artist{i}", scrobbleArray[i].ArtistName);
        parameters.Add($"track{i}", scrobbleArray[i].TrackName);
        parameters.Add($"timestamp{i}", scrobbleArray[i].Timestamp.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture));

        // optional params
        if (!string.IsNullOrEmpty(scrobbleArray[i].AlbumName))
          parameters.Add($"album{i}", scrobbleArray[i].AlbumName!);
        if (!string.IsNullOrEmpty(scrobbleArray[i].AlbumArtistName))
          parameters.Add($"albumArtist{i}", scrobbleArray[i].AlbumArtistName!);
        if (scrobbleArray[i].Duration != null)
          parameters.Add($"duration{i}", scrobbleArray[i].Duration!.Value.Seconds.ToString(System.Globalization.CultureInfo.InvariantCulture));
        if (scrobbleArray[i].TrackNumber != null)
          parameters.Add($"trackNumber{i}", scrobbleArray[i].TrackNumber!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        if (!string.IsNullOrEmpty(scrobbleArray[i].Mbid))
          parameters.Add($"mbid{i}", scrobbleArray[i].Mbid!);
        if (scrobbleArray[i].ChosenByUser != null)
          parameters.Add($"chosenByUser{i}", scrobbleArray[i].ChosenByUser!.Value ? "1" : "0");
        if (!string.IsNullOrEmpty(scrobbleArray[i].Context))
          parameters.Add($"context{i}", scrobbleArray[i].Context!);
      }

      var result = await _invoker.SendAsync("track.scrobble", parameters, true, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<ScrobbleInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var scrobbleArrayProperty = result.Data.RootElement.GetProperty("scrobbles").TryGetProperty("scrobble", out var ta) ? ta : default;
        var scrobblesInfo = JsonHelper.MakeListFromJsonArray(scrobbleArrayProperty, ScrobbleInfo.FromJson);

        return ApiResult<IReadOnlyList<ScrobbleInfo>>.Success(scrobblesInfo);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<ScrobbleInfo>>.Failure(null, result.HttpStatus, "Failed to parse scrobble info list: " + ex.Message);
      }
    }
  }
}