using Shoegaze.LastFM.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Artist
{
  public class ArtistApi : IArtistApi
  {
    private readonly ILastfmRequestInvoker _invoker;

    internal ArtistApi(ILastfmRequestInvoker invoker) => _invoker = invoker;

    public async Task<ApiResult<ArtistInfo>> GetInfoByNameAsync(string artistName, string? username = null, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["artist"] = artistName,
      };

      return await GetInfoAsync(parameters, username, autoCorrect, ct);
    }

    public async Task<ApiResult<ArtistInfo>> GetInfoByMbidAsync(string mbid, string? username = null, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["mbid"] = mbid,
      };

      return await GetInfoAsync(parameters, username, autoCorrect, ct);
    }

    private async Task<ApiResult<ArtistInfo>> GetInfoAsync(Dictionary<string, string> parameters, string? username, bool autoCorrect = true, CancellationToken ct = default)
    {
      if (username != null)
        parameters["username"] = username;
      parameters.Add("autocorrect", autoCorrect ? "1" : "0");

      var result = await _invoker.SendAsync("artist.getInfo", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<ArtistInfo>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var artistInfo = ArtistInfo.FromJson(result.Data.RootElement.GetProperty("artist"));
        if (username == null)
          artistInfo.UserPlayCount = null; // usercount may have the same json property name as plays; if user is null userplaycount should be null

        return ApiResult<ArtistInfo>.Success(artistInfo);
      }
      catch (Exception ex)
      {
        return ApiResult<ArtistInfo>.Failure(null, result.HttpStatus, "Failed to parse artist info: " + ex.Message);
      }
    }
  }
}