using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Album
{
  public interface IAlbumApi
  {
    Task<ApiResult<AlbumInfo>> GetInfoByNameAsync(
      string albumName,
      string artistName,
      string? username = null,
      bool autoCorrect = true,
      string? language = null,
      CancellationToken ct = default);

    Task<ApiResult<AlbumInfo>> GetInfoByMbidAsync(
      string mbid,
      string? username = null,
      bool autoCorrect = true,
      string? language = null,
      CancellationToken ct = default);
  }
}