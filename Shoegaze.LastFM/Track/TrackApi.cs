using Shoegaze.LastFM.Tag;

namespace Shoegaze.LastFM.Track
{
  public class TrackApi : ITrackApi
  {
    private readonly ILastfmRequestInvoker _invoker;

    internal TrackApi(ILastfmRequestInvoker invoker) => _invoker = invoker;

    public async Task<ApiResult<TrackInfo>> GetInfoByNameAsync(string track, string artist, string? username = null, bool autocorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["track"] = track,
        ["artist"] = artist,
        ["autocorrect"] = autocorrect ? "1" : "0"
      };

      if (!string.IsNullOrWhiteSpace(username))
        parameters["username"] = username;

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

    public async Task<ApiResult<TrackInfo>> GetInfoByMbidAsync(string mbid, string? username = null, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["mbid"] = mbid
      };

      if (!string.IsNullOrWhiteSpace(username))
        parameters["username"] = username;

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
        return ApiResult<TrackInfo>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse track info: " + ex.Message);
      }
    }

    public async Task<ApiResult<TrackInfo>> GetCorrectionAsync(string track, string artist, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["track"] = track,
        ["artist"] = artist
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
        return ApiResult<TrackInfo>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse track info: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarByNameAsync(string track, string artist, bool autocorrect = true, int? limit = null, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["track"] = track,
        ["artist"] = artist,
        ["autocorrect"] = autocorrect ? "1" : "0"
      };

      if (limit.HasValue)
        parameters["limit"] = limit.Value.ToString();

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
        return ApiResult<IReadOnlyList<TrackInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse similar track list: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<TrackInfo>>> GetSimilarByMbidAsync(string mbid, bool autocorrect = true, int? limit = null, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["mbid"] = mbid,
        ["autocorrect"] = autocorrect ? "1" : "0"
      };

      if (limit.HasValue)
        parameters["limit"] = limit.Value.ToString();

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
        return ApiResult<IReadOnlyList<TrackInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse similar track list: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetUserTagsByName(string track, string artist, string? username = null, bool autocorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["track"] = track,
        ["artist"] = artist,
        ["autocorrect"] = autocorrect ? "1" : "0"
      };

      return await GetUserTags(parameters, username, ct);
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetUserTagsByMbid(string mbid, string? username = null, bool autocorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["mbid"] = mbid,
        ["autocorrect"] = autocorrect ? "1" : "0"
      };

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
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse track tag list: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByName(string track, string artist, bool autocorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["track"] = track,
        ["artist"] = artist,
        ["autocorrect"] = autocorrect ? "1" : "0"
      };

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
          tag.UserUsedCount = null; // not used in this function, but json property has same name as count

        return ApiResult<IReadOnlyList<TagInfo>>.Success(tags);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse track tag list: " + ex.Message);
      }
    }

    public async Task<ApiResult<PagedResult<TrackInfo>>> SearchAsync(string track, string? artist = null, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("track", track);

      if (artist != null)
        parameters["artist"] = artist;

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
        return ApiResult<PagedResult<TrackInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse tracks: " + ex.Message);
      }
    }
  }
}