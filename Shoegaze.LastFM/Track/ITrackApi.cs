using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Track
{
  public interface ITrackApi
  {
    Task<ApiResult<TrackInfo>> GetInfoByNameAsync(
      string track,
      string artist,
      string? username = null,
      bool autocorrect = false,
      CancellationToken ct = default);

    Task<ApiResult<TrackInfo>> GetInfoByMbidAsync(
      string mbid,
      string? username = null,
      CancellationToken ct = default);
  }
}
