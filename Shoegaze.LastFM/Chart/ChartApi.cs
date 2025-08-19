using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Chart
{
  /// <summary>
  /// Access to chart-related api endpoints.
  /// </summary>
  public class ChartApi : IChartApi
  {
    private readonly ILastfmApiInvoker _invoker;

    internal ChartApi(ILastfmApiInvoker invoker)
    {
      _invoker = invoker;
    }

    /// <summary>
    /// Get the global top artists.
    /// </summary>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of the global top artists, or error information.</returns>
    public async Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);

      var result = await _invoker.SendAsync("chart.getTopArtists", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<ArtistInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var artistsProp = result.Data.RootElement.GetProperty("artists");
        var artistArray = artistsProp.TryGetProperty("artist", out var ta) ? ta : default;
        var artists = JsonHelper.MakeListFromJsonArray(artistArray, ArtistInfo.FromJson);

        foreach (var artist in artists)
          artist.UserPlayCount = null;

        return ApiResult<PagedResult<ArtistInfo>>.Success(PagedResult<ArtistInfo>.FromJson(artistsProp, artists));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<ArtistInfo>>.Failure(null, result.HttpStatus, $"Failed to parse top artists: {ex.Message}");
      }
    }

    /// <summary>
    /// Get the global top tags.
    /// </summary>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of the global top tags, or error information.</returns>
    public async Task<ApiResult<PagedResult<TagInfo>>> GetTopTagsAsync(int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);

      var result = await _invoker.SendAsync("chart.getTopTags", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<TagInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tagsProp = result.Data.RootElement.GetProperty("tags");
        var tagArray = tagsProp.TryGetProperty("tag", out var ta) ? ta : default;
        var tags = JsonHelper.MakeListFromJsonArray(tagArray, TagInfo.FromJson);

        foreach (var tag in tags)
        {
          tag.CountOnTrack = null;
          tag.UserUsedCount = null;
          tag.WeightOnAlbum = null;
        }

        return ApiResult<PagedResult<TagInfo>>.Success(PagedResult<TagInfo>.FromJson(tagsProp, tags));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<TagInfo>>.Failure(null, result.HttpStatus, $"Failed to parse top tags: {ex.Message}");
      }
    }

    /// <summary>
    /// Get the global top tracks.
    /// </summary>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of the global top tracks, or error information.</returns>
    public async Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);

      var result = await _invoker.SendAsync("chart.getTopTracks", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<TrackInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tracksProp = result.Data.RootElement.GetProperty("tracks");
        var trackArray = tracksProp.TryGetProperty("track", out var ta) ? ta : default;
        var tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

        foreach (var track in tracks)
          track.UserPlayCount = null;

        return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(tracksProp, tracks));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<TrackInfo>>.Failure(null, result.HttpStatus, $"Failed to parse top tracks: {ex.Message}");
      }
    }
  }
}