using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Tag
{
  public class TagApi : ITagApi
  {
    private readonly ILastfmRequestInvoker _invoker;

    internal TagApi(ILastfmRequestInvoker invoker) => _invoker = invoker;

    public async Task<ApiResult<TagInfo>> GetInfoAsync(string name, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["tag"] = name
      };

      var result = await _invoker.SendAsync("tag.getInfo", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<TagInfo>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tagInfo = TagInfo.FromJson(result.Data.RootElement.GetProperty("tag"));
        return ApiResult<TagInfo>.Success(tagInfo);
      }
      catch (Exception ex)
      {
        return ApiResult<TagInfo>.Failure(null, result.HttpStatus, "Failed to parse tag info: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetSimilarAsync(string name, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["tag"] = name
      };

      var result = await _invoker.SendAsync("tag.getSimilar", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tagArray = result.Data.RootElement.GetProperty("similartags").TryGetProperty("tag", out var ta) ? ta : default;
        var tags = JsonHelper.MakeListFromJsonArray(tagArray, TagInfo.FromJson);

        return ApiResult<IReadOnlyList<TagInfo>>.Success(tags);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse similar tag list: " + ex.Message);
      }
    }

    public async Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsAsync(string tagName, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("tag", tagName);

      var result = await _invoker.SendAsync("tag.getTopAlbums", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<AlbumInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var albumsProperty = result.Data.RootElement.GetProperty("albums");
        var albumArray = albumsProperty.TryGetProperty("album", out var ta) ? ta : default;

        var albums = JsonHelper.MakeListFromJsonArray(albumArray, AlbumInfo.FromJson);

        return ApiResult<PagedResult<AlbumInfo>>.Success(PagedResult<AlbumInfo>.FromJson(albumsProperty, albums));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<AlbumInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse albums: " + ex.Message);
      }
    }

    public async Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(string tagName, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("tag", tagName);

      var result = await _invoker.SendAsync("tag.getTopArtists", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<ArtistInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var artistsProperty = result.Data.RootElement.GetProperty("topartists");
        var artistArray = artistsProperty.TryGetProperty("artist", out var ta) ? ta : default;
        var artists = JsonHelper.MakeListFromJsonArray(artistArray, ArtistInfo.FromJson);

        return ApiResult<PagedResult<ArtistInfo>>.Success(PagedResult<ArtistInfo>.FromJson(artistsProperty, artists));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<ArtistInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse artists: " + ex.Message);
      }
    }

    /// <summary>
    /// Fetches the global top tags on Last.fm, sorted by number of times used.
    /// </summary>
    /// <param name="limit">Maximum items per page (maximum is 1000).</param>
    /// <param name="page">The page to get.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task<ApiResult<PagedResult<TagInfo>>> GetTopTagsAsync(int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>();
      if (page.HasValue)
        parameters["offset"] = (page.Value * (limit ?? 50)).ToString(); // 50 is default if num_res (limit) is not set
      if (limit.HasValue)
        parameters["num_res"] = limit.Value.ToString();

      var result = await _invoker.SendAsync("tag.getTopTags", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<TagInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var topTagProperty = result.Data.RootElement.GetProperty("toptags");
        var tagArray = topTagProperty.TryGetProperty("tag", out var ta) ? ta : default;
        var tags = JsonHelper.MakeListFromJsonArray(tagArray, TagInfo.FromJson);

        foreach (var tag in tags)
        {
          // same name as count property...
          tag.UserUsedCount = null;
          tag.CountOnTrack = null;
        }

        return ApiResult<PagedResult<TagInfo>>.Success(PagedResult<TagInfo>.FromJson(topTagProperty, tags));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<TagInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse tags: " + ex.Message);
      }
    }

    public async Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(string tagName, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("tag", tagName);

      var result = await _invoker.SendAsync("tag.getTopTracks", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<TrackInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tracksProperty = result.Data.RootElement.GetProperty("tracks");
        var trackArray = tracksProperty.TryGetProperty("track", out var ta) ? ta : default;
        var tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

        return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(tracksProperty, tracks));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<TrackInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse tracks: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<WeeklyChartInfo>>> GetWeeklyChartListAsync(string tagName, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["tag"] = tagName
      };

      var result = await _invoker.SendAsync("tag.getWeeklyChartList", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var chartArray = result.Data.RootElement.GetProperty("weeklychartlist").TryGetProperty("chart", out var ta) ? ta : default;
        var charts = JsonHelper.MakeListFromJsonArray(chartArray, WeeklyChartInfo.FromJson);

        return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Success(charts);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse weekly chart list: " + ex.Message);
      }
    }
  }
}