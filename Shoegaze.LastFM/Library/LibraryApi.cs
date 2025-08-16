using Shoegaze.LastFM.Artist;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Library
{
  public class LibraryApi : ILibraryApi
  {
    private readonly ILastfmApiInvoker _invoker;

    internal LibraryApi(ILastfmApiInvoker invoker)
    {
      _invoker = invoker;
    }

    public async Task<ApiResult<PagedResult<ArtistInfo>>> GetArtistsAsync(string username, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("user", username);

      var result = await _invoker.SendAsync("library.getArtists", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<ArtistInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var artistsProp = result.Data.RootElement.GetProperty("artists");
        var artistArray = artistsProp.TryGetProperty("artist", out var ta) ? ta : default;
        var artists = JsonHelper.MakeListFromJsonArray(artistArray, ArtistInfo.FromJson);

        foreach (var artist in artists)
          artist.PlayCount = null;

        return ApiResult<PagedResult<ArtistInfo>>.Success(PagedResult<ArtistInfo>.FromJson(artistsProp, artists));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<ArtistInfo>>.Failure(null, result.HttpStatus, $"Failed to parse top artists: {ex.Message}");
      }
    }
  }
}
