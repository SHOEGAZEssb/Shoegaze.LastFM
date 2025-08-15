using Shoegaze.LastFM.Tag;

namespace Shoegaze.LastFM.Track
{
  public class TrackApi : ITrackApi
  {
    private readonly ILastfmApiInvoker _invoker;

    internal TrackApi(ILastfmApiInvoker invoker) => _invoker = invoker;

    public async Task<ApiResult<TrackInfo>> GetInfoByNameAsync(string track, string artist, string? username = null, bool autocorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "track", track },
        { "artist", artist }
      };

      ParameterHelper.AddAutoCorrectParameter(parameters, autocorrect);

      return await GetInfoAsync(parameters, username, ct);
    }

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
        return ApiResult<TrackInfo>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

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

    public async Task<ApiResult<TrackInfo>> GetCorrectionAsync(string track, string artist, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "track", track },
        { "artist", artist }
      };

      var result = await _invoker.SendAsync("track.getCorrection", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<TrackInfo>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

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

    public async Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarByNameAsync(string track, string artist, bool autocorrect = true, int? limit = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, null);
      parameters.Add("track", track);
      parameters.Add("artist", artist);
      return await GetSimilarAsync(parameters, autocorrect, ct);
    }

    public async Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarByMbidAsync(string mbid, bool autocorrect = true, int? limit = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, null);
      parameters.Add("mbid", mbid);
      return await GetSimilarAsync(parameters, autocorrect, ct);
    }

    private async Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarAsync(Dictionary<string, string> parameters, bool autocorrect, CancellationToken ct)
    {
      ParameterHelper.AddAutoCorrectParameter(parameters, autocorrect);

      var result = await _invoker.SendAsync("track.getSimilar", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<TrackInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

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

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetUserTagsByName(string track, string artist, string? username = null, bool autocorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "track", track },
        { "artist", artist }
      };

      ParameterHelper.AddAutoCorrectParameter(parameters, autocorrect);

      return await GetUserTags(parameters, username, ct);
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetUserTagsByMbid(string mbid, string? username = null, bool autocorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "mbid", mbid }
      };

      ParameterHelper.AddAutoCorrectParameter(parameters, autocorrect);

      return await GetUserTags(parameters, username, ct);
    }

    private async Task<ApiResult<IReadOnlyList<TagInfo>>> GetUserTags(Dictionary<string, string> parameters, string? username, CancellationToken ct)
    {
      if (username != null)
        parameters.Add("user", username);

      var result = await _invoker.SendAsync("track.getTags", parameters, username == null, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

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

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByName(string track, string artist, bool autocorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        { "track", track },
        { "artist", artist }
      };

      ParameterHelper.AddAutoCorrectParameter(parameters, autocorrect);

      return await GetTopTags(parameters, ct);
    }

    private async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTags(Dictionary<string, string> parameters, CancellationToken ct)
    {
      var result = await _invoker.SendAsync("track.getTopTags", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

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

    public async Task<ApiResult<PagedResult<TrackInfo>>> SearchAsync(string track, string? artist = null, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("track", track);

      if (artist != null)
        parameters.Add("artist", artist);

      var result = await _invoker.SendAsync("track.search", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<TrackInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

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

    public async Task<ApiResult> AddTagsAsync(string trackName, string artistName, string tag, CancellationToken ct = default)
    {
      return await AddTagsAsync(trackName, artistName, [tag], ct);
    }

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
        return ApiResult.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      return ApiResult.Success();
    }

    public async Task<ApiResult> RemoveTagsAsync(string trackName, string artistName, string tag, CancellationToken ct = default)
    {
      return await RemoveTagsAsync(trackName, artistName, [tag], ct);
    }

    public async Task<ApiResult> RemoveTagsAsync(string trackName, string artistName, IEnumerable<string> tags, CancellationToken ct = default)
    {
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
          return ApiResult.Failure(result.Status, result.HttpStatus, result.ErrorMessage);
      }

      return ApiResult.Success();
    }

    public async Task<ApiResult> SetLoveState(string trackName, string artistName, bool loveState, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["track"] = trackName,
        ["artist"] = artistName
      };

      var result = await _invoker.SendAsync(loveState ? "track.love" : "track.unlove", parameters, true, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      return ApiResult.Success();
    }

    public async Task<ApiResult<ScrobbleInfo>> UpdateNowPlayingAsync(string trackName, string artistName, string? albumName = null, string? albumArtistName = null, CancellationToken ct = default)
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
        return ApiResult<ScrobbleInfo>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var resultsProperty = result.Data.RootElement.GetProperty("nowplaying");
        var scrobbleInfo = ScrobbleInfo.FromJson(resultsProperty);
        return ApiResult<ScrobbleInfo>.Success(scrobbleInfo);
      }
      catch (Exception ex)
      {
        return ApiResult<ScrobbleInfo>.Failure(null, result.HttpStatus, "Failed to parse scrobble info: " + ex.Message);
      }
    }
  }
}