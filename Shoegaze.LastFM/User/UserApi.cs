using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
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
      return ApiResult<UserInfo>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

    try
    {
      var root = result.Data.RootElement.GetProperty("user");
      return ApiResult<UserInfo>.Success(UserInfo.FromJson(root));
    }
    catch (Exception ex)
    {
      return ApiResult<UserInfo>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse user info: " + ex.Message);
    }
  }
  public async Task<ApiResult<PagedResult<UserInfo>>> GetFriendsAsync(string? username = null, bool includeRecentTracks = false, int? page = null, int? limit = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
    var requireAuth = string.IsNullOrWhiteSpace(username);

    if (!requireAuth)
      parameters["user"] = username!;

    // todo: check if recenttracks is actually supported or deprecated
    if (includeRecentTracks)
      parameters["recenttracks"] = "1";

    var result = await _invoker.SendAsync("user.getFriends", parameters, requireAuth, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<UserInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

    try
    {
      var friendsElement = result.Data.RootElement.GetProperty("friends");

      // handle missing "user" key (zero friends)
      IReadOnlyList<UserInfo> friends;
      if (friendsElement.TryGetProperty("user", out var userElement))
        friends = JsonHelper.MakeListFromJsonArray(userElement, UserInfo.FromJson);
      else
        friends = [];

      return ApiResult<PagedResult<UserInfo>>.Success(PagedResult<UserInfo>.FromJson(friendsElement, friends));
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<UserInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse friends list: " + ex.Message);
    }
  }

  public async Task<ApiResult<PagedResult<TrackInfo>>> GetLovedTracksAsync(
  string? username = null,
  int? page = null,
  int? limit = null,
  CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);

    var requireAuth = string.IsNullOrWhiteSpace(username);
    if (!requireAuth)
      parameters["user"] = username!;

    var result = await _invoker.SendAsync("user.getLovedTracks", parameters, requireAuth, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<TrackInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

    try
    {
      var lovedTracks = result.Data.RootElement.GetProperty("lovedtracks");
      var trackArray = lovedTracks.TryGetProperty("track", out var ta) ? ta : default; 
      var tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

      foreach (var t in tracks)
        t.UserLoved = true;

      return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(lovedTracks, tracks));
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<TrackInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, $"Failed to parse loved tracks: {ex.Message}");
    }
  }

  public async Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(string? username = null, TimePeriod? period = null, int? limit = null, int? page = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);

    var requireAuth = string.IsNullOrWhiteSpace(username);
    if (!requireAuth)
      parameters["user"] = username!;
    if (period.HasValue)
      parameters["period"] = period.Value.ToApiString();

    var result = await _invoker.SendAsync("user.getTopTracks", parameters, requireAuth, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<TrackInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

    try
    {
      var topTracksElement = result.Data.RootElement.GetProperty("toptracks");
      var trackArray = topTracksElement.TryGetProperty("track", out var te) ? te : default;
      var tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

      foreach (var track in tracks)
        track.PlayCount = null; // userplaycount and playcount have the same json property name; in this case only userplaycount is valid

      return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(topTracksElement, tracks));
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<TrackInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse top tracks: " + ex.Message);
    }
  }

  public async Task<ApiResult<PagedResult<TrackInfo>>> GetRecentTracksAsync(string? username = null, bool? extended = null, DateTime? from = null, DateTime? to = null, int? limit = null, int? page = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);

    var requireAuth = string.IsNullOrWhiteSpace(username);
    if (!requireAuth)
      parameters["user"] = username!;
    if (extended != null)
      parameters["extended"] = extended.Value ? "1" : "0";
    if (from != null)
      parameters["from"] = new DateTimeOffset(from.Value.ToUniversalTime()).ToUnixTimeSeconds().ToString();
    if (to != null)
      parameters["to"] = new DateTimeOffset(to.Value.ToUniversalTime()).ToUnixTimeSeconds().ToString();

    var result = await _invoker.SendAsync("user.getRecentTracks", parameters, requireAuth, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<TrackInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

    try
    {
      var recentTracksElement = result.Data.RootElement.GetProperty("recenttracks");
      var trackArray = recentTracksElement.TryGetProperty("track", out var te) ? te : default;
      var tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

      return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(recentTracksElement, tracks));
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<TrackInfo>>.Failure(
        LastFmStatusCode.UnknownError,
        result.HttpStatus,
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
      return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

    try
    {
      var tagArray = result.Data.RootElement.GetProperty("toptags").TryGetProperty("tag", out var ta) ? ta : default;
      var tags = JsonHelper.MakeListFromJsonArray(tagArray, TagInfo.FromJson);

      return ApiResult<IReadOnlyList<TagInfo>>.Success(tags);
    }
    catch (Exception ex)
    {
      return ApiResult<IReadOnlyList<TagInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse top tags: " + ex.Message);
    }
  }

  public async Task<ApiResult<IReadOnlyList<T>>> GetPersonalTagsAsync<T>(string username, string tag, int? limit = null, int? page = null, CancellationToken ct = default) where T : ITagable
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
    parameters.Add("user", username);
    parameters.Add("tag", tag);
    parameters.Add("taggingtype", GetTypeJsonPropertyName(typeof(T)).ToLower());

    var iTagablePropertyName = GetTypeJsonPropertyName(typeof(T));
    var result = await _invoker.SendAsync($"user.getPersonalTags", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<IReadOnlyList<T>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

    try
    {
      var chartArray = result.Data.RootElement.GetProperty($"taggings").GetProperty($"{iTagablePropertyName.ToLower()}s").TryGetProperty(iTagablePropertyName.ToLower(), out var ta) ? ta : default;
      var charts = chartArray.ValueKind switch
      {
        JsonValueKind.Array => [.. chartArray.EnumerateArray().Select(ITagableFromJson<T>)],
        JsonValueKind.Object => [ITagableFromJson<T>(chartArray)],
        _ => new List<T>()
      };

      return ApiResult<IReadOnlyList<T>>.Success(charts);
    }
    catch (Exception ex)
    {
      return ApiResult<IReadOnlyList<T>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, $"Failed to parse personal tags: " + ex.Message);
    }
  }

  public async Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(string username, TimePeriod? period = null, int? limit = null, int? page = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
    parameters.Add("user", username);

    if (period.HasValue)
      parameters["period"] = period.Value.ToApiString();

    var result = await _invoker.SendAsync("user.getTopArtists", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<ArtistInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

    try
    {
      var topArtistsProperty = result.Data.RootElement.GetProperty("topartists");
      var artistArray = topArtistsProperty.TryGetProperty("artist", out var ta) ? ta : default;
      var artists = JsonHelper.MakeListFromJsonArray(artistArray, ArtistInfo.FromJson);

      foreach (var artist in artists)
        artist.PlayCount = null; // userplaycount and playcount have the same property name; in this case only userplaycount is valid

      return ApiResult<PagedResult<ArtistInfo>>.Success(PagedResult<ArtistInfo>.FromJson(topArtistsProperty, artists));
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<ArtistInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse top artists: " + ex.Message);
    }
  }

  public async Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsAsync(string username, TimePeriod? period = null, int? limit = null, int? page = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
    parameters.Add("user", username);

    if (period.HasValue)
      parameters["period"] = period.Value.ToApiString();

    var result = await _invoker.SendAsync("user.getTopAlbums", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<AlbumInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

    try
    {
      var topAlbumsProperty = result.Data.RootElement.GetProperty("topalbums");
      var albumsArray = topAlbumsProperty.TryGetProperty("album", out var ta) ? ta : default;
      var albums = JsonHelper.MakeListFromJsonArray(albumsArray, AlbumInfo.FromJson);

      foreach (var album in albums)
        album.PlayCount = null; // same property name as UserPlayCount

      return ApiResult<PagedResult<AlbumInfo>>.Success(PagedResult<AlbumInfo>.FromJson(topAlbumsProperty, albums));
    }
    catch (Exception ex)
    {
      return ApiResult<PagedResult<AlbumInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse top albums: " + ex.Message);
    }
  }

  public async Task<ApiResult<IReadOnlyList<WeeklyChartInfo>>> GetWeeklyChartListAsync(string username, CancellationToken ct = default)
  {
    var parameters = new Dictionary<string, string>
    {
      ["user"] = username
    };

    var result = await _invoker.SendAsync("user.getWeeklyChartList", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

    try
    {
      var chartArray = result.Data.RootElement.GetProperty("weeklychartlist").TryGetProperty("chart", out var ta) ? ta : default;
      var charts = JsonHelper.MakeListFromJsonArray(chartArray, WeeklyChartInfo.FromJson);

      return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Success(charts);
    }
    catch (Exception ex)
    {
      return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, "Failed to parse weekly chart list: " + ex.Message);
    }
  }

  public async Task<ApiResult<IReadOnlyList<T>>> GetWeeklyChartAsync<T>(string username, DateTime? from = null, DateTime? to = null, CancellationToken ct = default) where T : IChartable
  {
    var parameters = new Dictionary<string, string>
    {
      ["user"] = username
    };

    if (from != null)
      parameters["from"] = new DateTimeOffset(from.Value.ToUniversalTime()).ToUnixTimeSeconds().ToString();
    if (to != null)
      parameters["to"] = new DateTimeOffset(to.Value.ToUniversalTime()).ToUnixTimeSeconds().ToString();

    var iChartablePropertyName = GetTypeJsonPropertyName(typeof(T));
    var result = await _invoker.SendAsync($"user.getWeekly{iChartablePropertyName}Chart", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<IReadOnlyList<T>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

    try
    {
      var chartArray = result.Data.RootElement.GetProperty($"weekly{iChartablePropertyName.ToLower()}chart").TryGetProperty(iChartablePropertyName.ToLower(), out var ta) ? ta : default;
      var charts = chartArray.ValueKind switch
      {
        JsonValueKind.Array => [.. chartArray.EnumerateArray().Select(IChartableFromJson<T>)],
        JsonValueKind.Object => [IChartableFromJson<T>(chartArray)],
        _ => new List<T>()
      };

      return ApiResult<IReadOnlyList<T>>.Success(charts);
    }
    catch (Exception ex)
    {
      return ApiResult<IReadOnlyList<T>>.Failure(LastFmStatusCode.UnknownError, result.HttpStatus, $"Failed to parse weekly {iChartablePropertyName} chart: " + ex.Message);
    }
  }

  internal static T ITagableFromJson<T>(JsonElement root) where T : ITagable
  {
    if (typeof(T) == typeof(AlbumInfo))
      return (T)(object)AlbumInfo.FromJson(root);
    else if (typeof(T) == typeof(ArtistInfo))
      return (T)(object)ArtistInfo.FromJson(root);
    else if (typeof(T) == typeof(TrackInfo))
      return (T)(object)TrackInfo.FromJson(root);

    throw new NotSupportedException($"No FromJson defined for type {typeof(T)}");
  }

  internal static string GetTypeJsonPropertyName(Type type)
  {
    return type switch
    {
      var t when t == typeof(ArtistInfo) => "Artist",
      var t when t == typeof(AlbumInfo) => "Album",
      var t when t == typeof(TrackInfo) => "Track",
      _ => throw new NotSupportedException($"Unsupported type: {type}")
    };
  }

  internal static T IChartableFromJson<T>(JsonElement root) where T : IChartable
  {
    if (typeof(T) == typeof(AlbumInfo))
      return (T)(object)AlbumInfo.FromJson(root);
    else if (typeof(T) == typeof(ArtistInfo))
      return (T)(object)ArtistInfo.FromJson(root);
    else if (typeof(T) == typeof(TrackInfo))
      return (T)(object)TrackInfo.FromJson(root);

    throw new NotSupportedException($"No FromJson defined for type {typeof(T)}");
  }
}