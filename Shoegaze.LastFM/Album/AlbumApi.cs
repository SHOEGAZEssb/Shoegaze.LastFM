using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Album
{
  public class AlbumApi : IAlbumApi
  {
    private readonly ILastfmApiInvoker _invoker;

    internal AlbumApi(ILastfmApiInvoker invoker) => _invoker = invoker;

    public async Task<ApiResult<AlbumInfo>> GetInfoByNameAsync(string albumName, string artistName, string? username = null, bool autoCorrect = true, string? language = null, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["album"] = albumName,
        ["artist"] = artistName
      };

      return await GetInfoAsync(parameters, username, autoCorrect, language, ct);
    }

    public async Task<ApiResult<AlbumInfo>> GetInfoByMbidAsync(string mbid, string? username = null, bool autoCorrect = true, string? language = null, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["mbid"] = mbid
      };

      return await GetInfoAsync(parameters, username, autoCorrect, language, ct);
    }

    private async Task<ApiResult<AlbumInfo>> GetInfoAsync(Dictionary<string, string> parameters, string? username = null, bool autoCorrect = true, string? language = null, CancellationToken ct = default)
    {
      if (username != null)
        parameters.Add("username", username);
      if (language != null)
        parameters.Add("lang", language);
      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      var result = await _invoker.SendAsync("album.getInfo", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<AlbumInfo>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var albumInfo = AlbumInfo.FromJson(result.Data.RootElement.GetProperty("album"));
        if (username == null)
          albumInfo.UserPlayCount = null; // usercount may have the same json property name as plays; if user is null userplaycount should be null

        return ApiResult<AlbumInfo>.Success(albumInfo);
      }
      catch (Exception ex)
      {
        return ApiResult<AlbumInfo>.Failure(null, result.HttpStatus, "Failed to parse album info: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByNameAsync(string albumName, string artistName, string? username = null, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["album"] = albumName,
        ["artist"] = artistName
      };

      return await GetTagsAsync(parameters, username, autoCorrect, ct);
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByMbidAsync(string mbid, string? username = null, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["mbid"] = mbid
      };

      return await GetTagsAsync(parameters, username, autoCorrect, ct);
    }

    private async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsAsync(Dictionary<string, string> parameters, string? username = null, bool autoCorrect = true, CancellationToken ct = default)
    {
      if (username != null)
        parameters.Add("username", username);
      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      var result = await _invoker.SendAsync("album.getTags", parameters, username == null, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tagArray = result.Data.RootElement.GetProperty("tags").TryGetProperty("tag", out var ta) ? ta : default;
        var tags = JsonHelper.MakeListFromJsonArray(tagArray, TagInfo.FromJson);

        return ApiResult<IReadOnlyList<TagInfo>>.Success(tags);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(null, result.HttpStatus, "Failed to parse tag info: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByNameAsync(string albumName, string artistName, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["album"] = albumName,
        ["artist"] = artistName
      };

      return await GetTopTagsAsync(parameters, autoCorrect, ct);
    }

    // currently broken?
    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByMbidAsync(string mbid, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["mbid"] = mbid
      };

      return await GetTopTagsAsync(parameters, autoCorrect, ct);
    }

    private async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsAsync(Dictionary<string, string> parameters, bool autoCorrect = true, CancellationToken ct = default)
    {
      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      var result = await _invoker.SendAsync("album.getTopTags", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tagArray = result.Data.RootElement.GetProperty("toptags").TryGetProperty("tag", out var ta) ? ta : default;
        var tags = JsonHelper.MakeListFromJsonArray(tagArray, TagInfo.FromJson);

        foreach (var tag in tags)
        {
          tag.UserUsedCount = null;
          tag.CountOnTrack = null;
          tag.Taggings = null;
        }

        return ApiResult<IReadOnlyList<TagInfo>>.Success(tags);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(null, result.HttpStatus, "Failed to parse tag info: " + ex.Message);
      }
    }
  }
}
