using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;
using System.Text.Json;

namespace Shoegaze.LastFM.User;

/// <summary>
/// Implements <see cref="IUserApi"/> using the Last.fm API.
/// </summary>
internal class UserApi : IUserApi
{
  private readonly ILastfmRequestInvoker _invoker;

  internal UserApi(ILastfmRequestInvoker invoker)
  {
    _invoker = invoker;
  }

  public async Task<ApiResult<UserInfo>> GetInfoAsync(string? username = null, CancellationToken ct = default)
  {
    var parameters = new Dictionary<string, string>();
    var requireAuth = string.IsNullOrWhiteSpace(username);

    if (!requireAuth)
      parameters["user"] = username!;

    var result = await _invoker.SendAsync("user.getInfo", parameters, requireAuth, ct);

    if (!result.IsSuccess || result.Data == null)
    {
      return ApiResult<UserInfo>.Failure(
          result.Status,
          result.HttpStatusCode,
          result.ErrorMessage
      );
    }

    try
    {
      var root = result.Data.RootElement.GetProperty("user");
      return ApiResult<UserInfo>.Success(UserInfo.FromJson(root), result.HttpStatusCode);
    }
    catch (Exception ex)
    {
      return ApiResult<UserInfo>.Failure(ApiStatusCode.UnknownError, result.HttpStatusCode, "Failed to parse user info: " + ex.Message);
    }
  }

  public async Task<ApiResult<PagedResult<UserInfo>>> GetFriendsAsync(string? username = null, int? page = null, int? limit = null, bool includeRecentTracks = false, CancellationToken ct = default)
  {
    var parameters = new Dictionary<string, string>();
    var requireAuth = string.IsNullOrWhiteSpace(username);

    if (!requireAuth)
      parameters["user"] = username!;

    if (page.HasValue)
      parameters["page"] = page.Value.ToString();
    if (limit.HasValue)
      parameters["limit"] = limit.Value.ToString();
    if (includeRecentTracks)
      parameters["recenttracks"] = "1";

    var result = await _invoker.SendAsync("user.getFriends", parameters, requireAuth, ct);

    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<UserInfo>>.Failure(result.Status, result.HttpStatusCode, result.ErrorMessage);

    try
    {
      var friendsElement = result.Data.RootElement.GetProperty("friends");

      // handle missing "user" key (zero friends)
      List<UserInfo> friends;
      if (friendsElement.TryGetProperty("user", out var userElement))
      {
        friends = userElement.ValueKind switch
        {
          JsonValueKind.Array => [.. userElement.EnumerateArray().Select(UserInfo.FromJson)],
          JsonValueKind.Object => [UserInfo.FromJson(userElement)],
          _ => []
        };
      }
      else
      {
        friends = [];
      }

      return ApiResult<PagedResult<UserInfo>>.Success(PagedResult<UserInfo>.FromJson(friendsElement, friends));
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<UserInfo>>.Failure(ApiStatusCode.UnknownError, result.HttpStatusCode, "Failed to parse friends list: " + ex.Message);
    }
  }

  public async Task<ApiResult<PagedResult<TrackInfo>>> GetLovedTracksAsync(
  string? username = null,
  int? page = null,
  int? limit = null,
  CancellationToken ct = default)
  {
    var parameters = new Dictionary<string, string>();
    var requireAuth = string.IsNullOrWhiteSpace(username);

    if (!requireAuth)
      parameters["user"] = username!;

    if (page.HasValue)
      parameters["page"] = page.Value.ToString();
    if (limit.HasValue)
      parameters["limit"] = limit.Value.ToString();

    var result = await _invoker.SendAsync("user.getLovedTracks", parameters, requireAuth, ct);

    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<TrackInfo>>.Failure(result.Status, result.HttpStatusCode, result.ErrorMessage);

    try
    {
      var root = result.Data.RootElement.GetProperty("lovedtracks");

      // Extract the track array
      var trackList = new List<TrackInfo>();
      if (root.TryGetProperty("track", out var trackElement))
      {
        switch (trackElement.ValueKind)
        {
          case JsonValueKind.Array:
            trackList = [.. trackElement.EnumerateArray().Select(TrackInfo.FromJson)];
            break;
          case JsonValueKind.Object:
            trackList = [TrackInfo.FromJson(trackElement)];
            break;
        }
      }

      return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(root, trackList));
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<TrackInfo>>.Failure(ApiStatusCode.UnknownError, result.HttpStatusCode, $"Failed to parse loved tracks: {ex.Message}");
    }
  }

  public async Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(string? username = null, TimePeriod? period = null, int? limit = null, int? page = null, CancellationToken ct = default)
  {
    var parameters = new Dictionary<string, string>();
    var requireAuth = string.IsNullOrWhiteSpace(username);

    if (!requireAuth)
      parameters["user"] = username!;

    if (period.HasValue)
      parameters["period"] = period.Value.ToApiString();
    if (limit.HasValue)
      parameters["limit"] = limit.Value.ToString();
    if (page.HasValue)
      parameters["page"] = page.Value.ToString();

    var result = await _invoker.SendAsync("user.getTopTracks", parameters, requireAuth, ct);

    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<TrackInfo>>.Failure(result.Status, result.HttpStatusCode, result.ErrorMessage);

    try
    {
      var topTracksElement = result.Data.RootElement.GetProperty("toptracks");

      var trackElement = topTracksElement.TryGetProperty("track", out var te) ? te : default;

      var tracks = trackElement.ValueKind switch
      {
        JsonValueKind.Array => [.. trackElement.EnumerateArray().Select(TrackInfo.FromJson)],
        JsonValueKind.Object => [TrackInfo.FromJson(trackElement)],
        _ => new List<TrackInfo>()
      };

      return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(topTracksElement, tracks));
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<TrackInfo>>.Failure(ApiStatusCode.UnknownError, result.HttpStatusCode, "Failed to parse top tracks: " + ex.Message);
    }
  }

  public async Task<ApiResult<PagedResult<TrackInfo>>> GetRecentTracksAsync(string? username = null, int? limit = null, int? page = null, CancellationToken ct = default)
  {
    var parameters = new Dictionary<string, string>();
    var requireAuth = string.IsNullOrWhiteSpace(username);

    if (!requireAuth)
      parameters["user"] = username!;

    if (page.HasValue)
      parameters["page"] = page.Value.ToString();
    if (limit.HasValue)
      parameters["limit"] = limit.Value.ToString();

    var result = await _invoker.SendAsync("user.getRecentTracks", parameters, requireAuth, ct);

    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<TrackInfo>>.Failure(result.Status, result.HttpStatusCode, result.ErrorMessage);

    try
    {
      var recentTracksElement = result.Data.RootElement.GetProperty("recenttracks");
      var trackElement = recentTracksElement.TryGetProperty("track", out var te) ? te : default;

      var tracks = trackElement.ValueKind switch
      {
        JsonValueKind.Array => [.. trackElement.EnumerateArray().Select(TrackInfo.FromJson)],
        JsonValueKind.Object => [TrackInfo.FromJson(trackElement)],
        _ => new List<TrackInfo>()
      };

      return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(recentTracksElement, tracks));
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<TrackInfo>>.Failure(
        ApiStatusCode.UnknownError,
        result.HttpStatusCode,
        "Failed to parse recent tracks: " + ex.Message);
    }
  }

  public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsAsync(string? username = null, int? limit = null, CancellationToken ct = default)
  {
    var parameters = new Dictionary<string, string>();
    var requireAuth = string.IsNullOrWhiteSpace(username);

    if (!requireAuth)
      parameters["user"] = username!;

    if (limit.HasValue)
      parameters["limit"] = limit.Value.ToString();

    var result = await _invoker.SendAsync("user.getTopTags", parameters, requireAuth, ct);

    if (!result.IsSuccess || result.Data == null)
      return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.Status, result.HttpStatusCode, result.ErrorMessage);

    try
    {
      var tagArray = result.Data.RootElement.GetProperty("toptags").TryGetProperty("tag", out var ta) ? ta : default;

      var tags = tagArray.ValueKind switch
      {
        JsonValueKind.Array => [.. tagArray.EnumerateArray().Select(TagInfo.FromJson)],
        JsonValueKind.Object => [TagInfo.FromJson(tagArray)],
        _ => new List<TagInfo>()
      };

      return ApiResult<IReadOnlyList<TagInfo>>.Success(tags, result.HttpStatusCode);
    }
    catch (Exception ex)
    {
      return ApiResult<IReadOnlyList<TagInfo>>.Failure(ApiStatusCode.UnknownError, result.HttpStatusCode, "Failed to parse top tags: " + ex.Message);
    }
  }

}