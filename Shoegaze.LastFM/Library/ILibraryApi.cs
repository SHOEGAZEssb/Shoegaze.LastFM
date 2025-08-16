using Shoegaze.LastFM.Artist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Library
{
  public interface ILibraryApi
  {
    public Task<ApiResult<PagedResult<ArtistInfo>>> GetArtistsAsync(
      string username,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);
  }
}