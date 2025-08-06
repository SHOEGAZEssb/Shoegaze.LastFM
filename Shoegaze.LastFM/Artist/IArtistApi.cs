using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Artist
{
  public interface IArtistApi
  {
    Task<ApiResult<ArtistInfo>> GetInfoByNameAsync(
      string artistName,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<ArtistInfo>> GetInfoByMbidAsync(
      string mbid,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<ArtistInfo>>> GetSimilarByNameAsync(
      string artist,
      bool autoCorrect = true,
      int? limit = null,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<ArtistInfo>>> GetSimilarByMbidAsync(
      string mbid,
      bool autoCorrect = true,
      int? limit = null,
      CancellationToken ct = default);

    Task<ApiResult<ArtistInfo>> GetCorrectionAsync(
      string artistName,
      CancellationToken ct = default);
  }
}
