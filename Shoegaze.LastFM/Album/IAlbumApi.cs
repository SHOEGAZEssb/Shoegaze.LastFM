using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
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

    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByNameAsync(
      string albumName,
      string artistName,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByMbidAsync(
      string mbid,
      string? username = null,
      bool autoCorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByNameAsync(
      string albumName,
      string artistName,
      bool autoCorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByMbidAsync(
      string mbid,
      bool autoCorrect = true,
      CancellationToken ct = default);

    Task<ApiResult<PagedResult<AlbumInfo>>> SearchAsync(
      string albumName,
      int? limit = null,
      int? page = null,
      CancellationToken ct = default);
  }
}