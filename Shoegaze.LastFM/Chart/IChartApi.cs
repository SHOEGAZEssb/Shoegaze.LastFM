using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Chart
{
  public interface IChartApi
  {
    Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    Task<ApiResult<PagedResult<TagInfo>>> GetTopTagsAsync(
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);

    Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);
  }
}