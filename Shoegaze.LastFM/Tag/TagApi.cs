using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Tag
{
  /// <summary>
  /// Access to tag-related api endpoints.
  /// </summary>
  public class TagApi : ITagApi
  {
    private readonly ILastfmApiInvoker _invoker;

    internal TagApi(ILastfmApiInvoker invoker) => _invoker = invoker;

    /// <summary>
    /// Get the metadata for a tag.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the tag metadata, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getInfo"/>.
    public async Task<ApiResult<TagInfo>> GetInfoAsync(string tagName, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["tag"] = tagName
      };

      var result = await _invoker.SendAsync("tag.getInfo", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<TagInfo>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

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

    /// <summary>
    /// Search for similar tags.
    /// Returns tags ranked by similarity, based on listening data.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of similar tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getSimilar"/>.
    [Obsolete("Currently broken on last.fms side, only returns an empty list.")]
    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetSimilarAsync(string tagName, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["tag"] = tagName
      };

      var result = await _invoker.SendAsync("tag.getSimilar", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tagArray = result.Data.RootElement.GetProperty("similartags").TryGetProperty("tag", out var ta) ? ta : default;
        var tags = JsonHelper.MakeListFromJsonArray(tagArray, TagInfo.FromJson);

        return ApiResult<IReadOnlyList<TagInfo>>.Success(tags);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(null, result.HttpStatus, "Failed to parse similar tag list: " + ex.Message);
      }
    }

    /// <summary>
    /// Get the top albums tagged by the given tag, ordered by tag count.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of albums, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getTopAlbums"/>.
    public async Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsAsync(string tagName, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("tag", tagName);

      var result = await _invoker.SendAsync("tag.getTopAlbums", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<AlbumInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var albumsProperty = result.Data.RootElement.GetProperty("albums");
        var albumArray = albumsProperty.TryGetProperty("album", out var ta) ? ta : default;

        var albums = JsonHelper.MakeListFromJsonArray(albumArray, AlbumInfo.FromJson);

        return ApiResult<PagedResult<AlbumInfo>>.Success(PagedResult<AlbumInfo>.FromJson(albumsProperty, albums));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<AlbumInfo>>.Failure(null, result.HttpStatus, "Failed to parse albums: " + ex.Message);
      }
    }

    /// <summary>
    /// Get the top artists tagged by the given tag, ordered by tag count.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of artists, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getTopArtists"/>.
    public async Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(string tagName, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("tag", tagName);

      var result = await _invoker.SendAsync("tag.getTopArtists", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<ArtistInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var artistsProperty = result.Data.RootElement.GetProperty("topartists");
        var artistArray = artistsProperty.TryGetProperty("artist", out var ta) ? ta : default;
        var artists = JsonHelper.MakeListFromJsonArray(artistArray, ArtistInfo.FromJson);

        return ApiResult<PagedResult<ArtistInfo>>.Success(PagedResult<ArtistInfo>.FromJson(artistsProperty, artists));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<ArtistInfo>>.Failure(null, result.HttpStatus, "Failed to parse artists: " + ex.Message);
      }
    }

    /// <summary>
    /// Get the top global tags, sorted by popularity (number of times used).
    /// </summary>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getTopTags"/>.
    public async Task<ApiResult<PagedResult<TagInfo>>> GetTopTagsAsync(int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>();
        if (page.HasValue)
          parameters["offset"] = (page.Value * (limit ?? 50)).ToString(System.Globalization.CultureInfo.InvariantCulture); // 50 is default if num_res (limit) is not set
        if (limit.HasValue)
          parameters["num_res"] = limit.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

      var result = await _invoker.SendAsync("tag.getTopTags", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<TagInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

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
          tag.WeightOnAlbum = null;
        }

        return ApiResult<PagedResult<TagInfo>>.Success(PagedResult<TagInfo>.FromJson(topTagProperty, tags));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<TagInfo>>.Failure(null, result.HttpStatus, "Failed to parse tags: " + ex.Message);
      }
    }

    /// <summary>
    /// Get the top tracks tagged by the given tag, ordered by tag count.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of tracks, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getTopTracks"/>.
    public async Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(string tagName, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("tag", tagName);

      var result = await _invoker.SendAsync("tag.getTopTracks", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<TrackInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tracksProperty = result.Data.RootElement.GetProperty("tracks");
        var trackArray = tracksProperty.TryGetProperty("track", out var ta) ? ta : default;
        var tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

        return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(tracksProperty, tracks));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<TrackInfo>>.Failure(null, result.HttpStatus, "Failed to parse tracks: " + ex.Message);
      }
    }

    /// <summary>
    /// Get a list of available charts for a tag.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the list of charts, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/tag.getWeeklyChartList"/>.
    public async Task<ApiResult<IReadOnlyList<WeeklyChartInfo>>> GetWeeklyChartListAsync(string tagName, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["tag"] = tagName
      };

      var result = await _invoker.SendAsync("tag.getWeeklyChartList", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var chartArray = result.Data.RootElement.GetProperty("weeklychartlist").TryGetProperty("chart", out var ta) ? ta : default;
        var charts = JsonHelper.MakeListFromJsonArray(chartArray, WeeklyChartInfo.FromJson);

        return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Success(charts);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Failure(null, result.HttpStatus, "Failed to parse weekly chart list: " + ex.Message);
      }
    }
  }
}