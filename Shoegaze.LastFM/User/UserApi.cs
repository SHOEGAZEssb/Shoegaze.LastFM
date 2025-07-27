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

  public async Task<ApiResult<PagedResult<UserInfo>>> GetFriendsAsync(
    string? username = null,
    int? page = null,
    int? limit = null,
    bool includeRecentTracks = false,
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

      var attr = friendsElement.GetProperty("@attr");

      var parsedPage = int.TryParse(attr.GetProperty("page").GetString(), out var p) ? p : 1;
      var totalPages = int.TryParse(attr.GetProperty("totalPages").GetString(), out var tp) ? tp : 1;
      var totalItems = int.TryParse(attr.GetProperty("total").GetString(), out var t) ? t : friends.Count;
      var perPage = int.TryParse(attr.GetProperty("perPage").GetString(), out var pp) ? pp : friends.Count;

      var paged = new PagedResult<UserInfo>
      {
        Items = friends,
        Page = parsedPage,
        TotalPages = totalPages,
        TotalItems = totalItems,
        PerPage = perPage
      };

      return ApiResult<PagedResult<UserInfo>>.Success(paged, result.HttpStatusCode);
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<UserInfo>>.Failure(ApiStatusCode.UnknownError, result.HttpStatusCode, "Failed to parse friends list: " + ex.Message);
    }
  }

  public async Task<ApiResult<PagedResult<LovedTrack>>> GetLovedTracksAsync(
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
      return ApiResult<PagedResult<LovedTrack>>.Failure(result.Status, result.HttpStatusCode, result.ErrorMessage);

    try
    {
      var root = result.Data.RootElement.GetProperty("lovedtracks");

      // Extract the track array
      var trackList = new List<LovedTrack>();
      if (root.TryGetProperty("track", out var trackElement))
      {
        switch (trackElement.ValueKind)
        {
          case JsonValueKind.Array:
            trackList = trackElement.EnumerateArray().Select(LovedTrack.FromJson).ToList();
            break;
          case JsonValueKind.Object:
            trackList = [LovedTrack.FromJson(trackElement)];
            break;
        }
      }

      // Parse pagination info
      var attr = root.GetProperty("@attr");
      var currentPage = int.TryParse(attr.GetProperty("page").GetString(), out var p) ? p : 1;
      var totalPages = int.TryParse(attr.GetProperty("totalPages").GetString(), out var tp) ? tp : 1;
      var totalItems = int.TryParse(attr.GetProperty("total").GetString(), out var t) ? t : trackList.Count;
      var perPage = int.TryParse(attr.GetProperty("perPage").GetString(), out var pp) ? pp : trackList.Count;

      var paged = new PagedResult<LovedTrack>
      {
        Items = trackList,
        Page = currentPage,
        TotalPages = totalPages,
        TotalItems = totalItems,
        PerPage = perPage
      };

      return ApiResult<PagedResult<LovedTrack>>.Success(paged, result.HttpStatusCode);
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<LovedTrack>>.Failure(ApiStatusCode.UnknownError, result.HttpStatusCode, $"Failed to parse loved tracks: {ex.Message}");
    }
  }

}