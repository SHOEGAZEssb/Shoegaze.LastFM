using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Geo
{
  public class GeoApi : IGeoApi
  {
    private readonly ILastfmApiInvoker _invoker;

    internal GeoApi(ILastfmApiInvoker invoker)
    {
      _invoker = invoker;
    }

    public async Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(string country, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("country", country);

      var result = await _invoker.SendAsync("geo.getTopArtists", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<ArtistInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

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

    public async Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(string country, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("country", country);

      var result = await _invoker.SendAsync("geo.getTopTracks", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<TrackInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

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
