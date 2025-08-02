using Shoegaze.LastFM.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Tag
{
  public class TagApi : ITagApi
  {
    private readonly ILastfmRequestInvoker _invoker;

    internal TagApi(ILastfmRequestInvoker invoker) => _invoker = invoker;

    public async Task<ApiResult<TagInfo>> GetInfoAsync(string name, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["tag"] = name
      };

      var result = await _invoker.SendAsync("tag.getInfo", parameters, false, ct);

      if (!result.IsSuccess || result.Data == null)
        return ApiResult<TagInfo>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tagInfo = TagInfo.FromJson(result.Data.RootElement.GetProperty("tag"));
        return ApiResult<TagInfo>.Success(tagInfo);
      }
      catch (Exception ex)
      {
        return ApiResult<TagInfo>.Failure(null, result.HttpStatus, "Failed to parse tag info: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetSimilarAsync(string name, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["tag"] = name
      };

      var result = await _invoker.SendAsync("tag.getSimilar", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tagArray = result.Data.RootElement.GetProperty("similartags").TryGetProperty("tag", out var ta) ? ta : default;

        var tracks = tagArray.ValueKind switch
        {
          JsonValueKind.Array => [.. tagArray.EnumerateArray().Select(TagInfo.FromJson)],
          JsonValueKind.Object => [TagInfo.FromJson(tagArray)],
          _ => new List<TagInfo>()
        };

        return ApiResult<IReadOnlyList<TagInfo>>.Success(tracks);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse similar tag list: " + ex.Message);
      }
    }
  }
}