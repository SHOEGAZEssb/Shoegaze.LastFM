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

      var user = new UserInfo
      {
        Id = root.GetProperty("id").GetString()!,
        Username = root.GetProperty("name").GetString()!,
        RealName = root.GetProperty("realname").GetString()!,
        Url = root.GetProperty("url").GetString()!,
        Country = root.GetProperty("country").GetString()!,
        Age = int.TryParse(root.GetProperty("age").GetString(), out var age) ? age : 0,
        Gender = root.GetProperty("gender").GetString()!,
        IsSubscriber = root.GetProperty("subscriber").GetString() == "1",
        Playcount = int.Parse(root.GetProperty("playcount").GetString()!),
        Playlists = int.Parse(root.GetProperty("playlists").GetString()!),
        RegisteredDate = DateTimeOffset
              .FromUnixTimeSeconds(long.Parse(root.GetProperty("registered").GetProperty("unixtime").GetString()!))
              .DateTime,
        ImageUrl = root.TryGetProperty("image", out var image) ? image.GetString() : null
      };

      return ApiResult<UserInfo>.Success(user, result.HttpStatusCode);
    }
    catch (Exception ex)
    {
      return ApiResult<UserInfo>.Failure(ApiStatusCode.UnknownError, result.HttpStatusCode, "Failed to parse user info: " + ex.Message);
    }
  }

}
