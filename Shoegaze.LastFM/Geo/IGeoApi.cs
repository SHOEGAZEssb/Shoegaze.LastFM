using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Geo
{
  public interface IGeoApi
  {
    public Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(
      string country,
      int? limit = null,
      int? page = null,
      CancellationToken ct  = default);

    public Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(
      string country,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);
  }
}
