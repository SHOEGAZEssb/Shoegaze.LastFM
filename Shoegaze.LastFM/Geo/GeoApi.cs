using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Geo
{
  /// <summary>
  /// Access to geo-related api endpoints.
  /// </summary>
  public class GeoApi : IGeoApi
  {
    private readonly ILastfmApiInvoker _invoker;

    internal GeoApi(ILastfmApiInvoker invoker)
    {
      _invoker = invoker;
    }

    /// <summary>
    /// Get the global top artists by country.
    /// </summary>
    /// <param name="country">A country name, as defined by the ISO 3166-1 country names standard.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top artists, or error information.</returns>
    public async Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(string country, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("country", country);

      var result = await _invoker.SendAsync("geo.getTopArtists", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<ArtistInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var artistsProp = result.Data.RootElement.GetProperty("topartists");
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
    /// Get the global top tracks by country.
    /// </summary>
    /// <param name="country">A country name, as defined by the ISO 3166-1 country names standard.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of top tracks, or error information.</returns>
    public async Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(string country, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("country", country);

      var result = await _invoker.SendAsync("geo.getTopTracks", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<TrackInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tracksProp = result.Data.RootElement.GetProperty("tracks");
        var trackArray = tracksProp.TryGetProperty("track", out var ta) ? ta : default;
        var tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

        foreach (var artist in tracks)
          artist.UserPlayCount = null;

        return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(tracksProp, tracks));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<TrackInfo>>.Failure(null, result.HttpStatus, $"Failed to parse top tracks: {ex.Message}");
      }
    }
  }
}
