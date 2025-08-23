using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;
using System.Text.Json;

namespace Shoegaze.LastFM.User;

/// <summary>
/// Access to user-related api endpoints.
/// </summary>
public class UserApi : IUserApi
{
  private readonly ILastfmApiInvoker _invoker;

  internal UserApi(ILastfmApiInvoker invoker)
  {
    _invoker = invoker;
  }

  /// <summary>
  /// Get information about a user profile.
  /// </summary>
  /// <param name="username">Username to look up. If null, uses the authenticated session.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>
  /// Result that contains the user info, or error information.
  /// </returns>
  /// <seealso href="https://www.last.fm/api/show/user.getInfo"/>.
  public async Task<ApiResult<UserInfo>> GetInfoAsync(string? username = null, CancellationToken ct = default)
  {
    var parameters = new Dictionary<string, string>();
    var requireAuth = string.IsNullOrWhiteSpace(username);

    if (!requireAuth)
      parameters["user"] = username!;

    var result = await _invoker.SendAsync("user.getInfo", parameters, requireAuth, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<UserInfo>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

    try
    {
      var root = result.Data.RootElement.GetProperty("user");
      return ApiResult<UserInfo>.Success(UserInfo.FromJson(root));
    }
    catch (Exception ex)
    {
      return ApiResult<UserInfo>.Failure(null, result.HttpStatus, "Failed to parse user info: " + ex.Message);
    }
  }

  /// <summary>
  /// Get a list of a user's Last.fm friends.
  /// </summary>
  /// <param name="username">Optional username. If null, uses the authenticated session.</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="includeRecentTracks">Whether to include recent track data per user.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>
  /// Result that contains a list with the users friends, or error information.
  /// </returns>
  /// <seealso href="https://www.last.fm/api/show/user.getFriends"/>.
  public async Task<ApiResult<PagedResult<UserInfo>>> GetFriendsAsync(string username, bool includeRecentTracks = false, int? limit = null, int ? page = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
    parameters.Add("user", username);

    // todo: check if recenttracks is actually supported or deprecated
    if (includeRecentTracks)
      parameters["recenttracks"] = "1";

    var result = await _invoker.SendAsync("user.getFriends", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<UserInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

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
      return ApiResult<PagedResult<UserInfo>>.Failure(null, result.HttpStatus, "Failed to parse friends list: " + ex.Message);
    }
  }

  /// <summary>
  /// Get the loved tracks of a user.
  /// </summary>
  /// <param name="username">User to get loved tracks for.</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of the loved tracks, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getLovedTracks"/>.
  public async Task<ApiResult<PagedResult<TrackInfo>>> GetLovedTracksAsync(string username, int? limit = null, int? page = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
    parameters.Add("user", username);

    var result = await _invoker.SendAsync("user.getLovedTracks", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<TrackInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

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
      return ApiResult<PagedResult<TrackInfo>>.Failure(null, result.HttpStatus, $"Failed to parse loved tracks: {ex.Message}");
    }
  }

  /// <summary>
  /// Get the top tracks listened to by a user.
  /// </summary>
  /// <param name="username">User to get top tracks for.</param>
  /// <param name="period">Time period (defaults to <see cref="TimePeriod.Overall"/>).</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of the top tracks, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getTopTracks"/>.
  public async Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(string username, TimePeriod? period = null, int? limit = null, int? page = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
    parameters.Add("user", username);

    if (period.HasValue)
      parameters["period"] = period.Value.ToApiString();

    var result = await _invoker.SendAsync("user.getTopTracks", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<TrackInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

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
      return ApiResult<PagedResult<TrackInfo>>.Failure(null, result.HttpStatus, "Failed to parse top tracks: " + ex.Message);
    }
  }

  /// <summary>
  /// Get a users recently played tracks.
  /// </summary>
  /// <param name="username">User to get recent tracks for.</param>
  /// <param name="extended">Wether to include extended data in each artist,
  /// and whether or not the user has loved each track.</param>
  /// <param name="fromDate">Beginning timestamp of a range - only fetch scrobbles after this time.</param>
  /// <param name="toDate"> End timestamp of a range - only fetch scrobbles before this time.</param>
  /// <param name="ignoreNowPlaying">Wether the currently "now playing" track should be filtered out in the result.</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of the recent tracks, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getRecentTracks"/>.
  public async Task<ApiResult<PagedResult<TrackInfo>>> GetRecentTracksAsync(string username, bool? extended = null, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null, bool ignoreNowPlaying = false, int? limit = null, int? page = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
    parameters.Add("user", username);

    if (extended != null)
      parameters["extended"] = extended.Value ? "1" : "0";
    if (fromDate != null)
      parameters["from"] = fromDate.Value.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture);
    if (toDate != null)
      parameters["to"] = toDate.Value.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture);

    var result = await _invoker.SendAsync("user.getRecentTracks", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<TrackInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

    try
    {
      var recentTracksElement = result.Data.RootElement.GetProperty("recenttracks");
      var trackArray = recentTracksElement.TryGetProperty("track", out var te) ? te : default;
      var tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

      if (ignoreNowPlaying)
        tracks = [.. tracks.Where(t => !t.IsNowPlaying)];

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

  /// <summary>
  /// Get the top tags used by a user.
  /// </summary>
  /// <param name="username">User to get top tags for.</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of top tags, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getTopTags"/>.
  public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsAsync(string username, int? limit = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, null);
    parameters.Add("user", username);

    var result = await _invoker.SendAsync("user.getTopTags", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

    try
    {
      var tagArray = result.Data.RootElement.GetProperty("toptags").TryGetProperty("tag", out var ta) ? ta : default;
      var tags = JsonHelper.MakeListFromJsonArray(tagArray, TagInfo.FromJson);

      return ApiResult<IReadOnlyList<TagInfo>>.Success(tags);
    }
    catch (Exception ex)
    {
      return ApiResult<IReadOnlyList<TagInfo>>.Failure(null, result.HttpStatus, "Failed to parse top tags: " + ex.Message);
    }
  }

  /// <summary>
  /// Get the users taggings.
  /// </summary>
  /// <typeparam name="T">The type of object to get the users taggings for.
  /// Supports <see cref="ArtistInfo"/>, <see cref="TrackInfo"/> and <see cref="AlbumInfo"/>.</typeparam>
  /// <param name="username">User whose taggings to get.</param>
  /// <param name="tag">Tag to get taggings for.</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of taggings, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getPersonalTags"/>.
  public async Task<ApiResult<IReadOnlyList<T>>> GetPersonalTagsAsync<T>(string username, string tag, int? limit = null, int? page = null, CancellationToken ct = default) where T : IUserTagable
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
    parameters.Add("user", username);
    parameters.Add("tag", tag);
    parameters.Add("taggingtype", GetTypeJsonPropertyName(typeof(T)).ToLower(System.Globalization.CultureInfo.CurrentCulture));

    var result = await _invoker.SendAsync($"user.getPersonalTags", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<IReadOnlyList<T>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

    try
    {
      var iTagablePropertyName = GetTypeJsonPropertyName(typeof(T));
      var chartArray = result.Data.RootElement.GetProperty($"taggings").GetProperty($"{iTagablePropertyName.ToLower(System.Globalization.CultureInfo.CurrentCulture)}s").TryGetProperty(iTagablePropertyName.ToLower(System.Globalization.CultureInfo.CurrentCulture), out var ta) ? ta : default;
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
      return ApiResult<IReadOnlyList<T>>.Failure(null, result.HttpStatus, $"Failed to parse personal tags: " + ex.Message);
    }
  }

  /// <summary>
  /// Get the top artists listened to by a user.
  /// </summary>
  /// <param name="username">User to get top artists for.</param>
  /// <param name="period">Time period (defaults to <see cref="TimePeriod.Overall"/>).</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of top artists, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getTopArtists"/>.
  public async Task<ApiResult<PagedResult<ArtistInfo>>> GetTopArtistsAsync(string username, TimePeriod? period = null, int? limit = null, int? page = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
    parameters.Add("user", username);

    if (period.HasValue)
      parameters["period"] = period.Value.ToApiString();

    var result = await _invoker.SendAsync("user.getTopArtists", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<ArtistInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

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
      return ApiResult<PagedResult<ArtistInfo>>.Failure(null, result.HttpStatus, "Failed to parse top artists: " + ex.Message);
    }
  }

  /// <summary>
  /// Get the top albums listened to by a user.
  /// </summary>
  /// <param name="username">User to get top albums for.</param>
  /// <param name="period">Time period (defaults to <see cref="TimePeriod.Overall"/>).</param>
  /// <param name="limit">Number of results per page (defaults to 50).</param>
  /// <param name="page">Page number (defaults to first page).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of top albums, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getTopAlbums"/>.
  public async Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsAsync(string username, TimePeriod? period = null, int? limit = null, int? page = null, CancellationToken ct = default)
  {
    var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
    parameters.Add("user", username);

    if (period.HasValue)
      parameters["period"] = period.Value.ToApiString();

    var result = await _invoker.SendAsync("user.getTopAlbums", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<PagedResult<AlbumInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

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
      return ApiResult<PagedResult<AlbumInfo>>.Failure(null, result.HttpStatus, "Failed to parse top albums: " + ex.Message);
    }
  }

  /// <summary>
  /// Get a list of available charts for a user.
  /// </summary>
  /// <param name="username">User to get the chart list for.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains the list of charts, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getWeeklyChartList"/>.
  public async Task<ApiResult<IReadOnlyList<WeeklyChartInfo>>> GetWeeklyChartListAsync(string username, CancellationToken ct = default)
  {
    var parameters = new Dictionary<string, string>
    {
      ["user"] = username
    };

    var result = await _invoker.SendAsync("user.getWeeklyChartList", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

    try
    {
      var chartArray = result.Data.RootElement.GetProperty("weeklychartlist").TryGetProperty("chart", out var ta) ? ta : default;
      var charts = JsonHelper.MakeListFromJsonArray(chartArray, WeeklyChartInfo.FromJson);

      return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Success(charts);
    }
    catch (Exception ex)
    {
      return ApiResult<IReadOnlyList<WeeklyChartInfo>>.Failure(null, result.HttpStatus, "Failed to parse weekly chart list: " + ex.Message);
    }
  }

  /// <summary>
  /// Get a weekly chart for a user.
  /// </summary>
  /// <typeparam name="T">The type of object to get the weekly chart for.
  /// Supports <see cref="ArtistInfo"/>, <see cref="TrackInfo"/> and <see cref="AlbumInfo"/>.</typeparam>
  /// <param name="username">User to get weekly chart for.</param>
  /// <param name="fromDate">The date at which the chart should start from.</param>
  /// <param name="toDate">The date at which the chart should end on</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Result that contains a list of objects for the chart, or error information.</returns>
  /// <seealso href="https://www.last.fm/api/show/user.getWeeklyArtistChart"/>.
  /// <seealso href="https://www.last.fm/api/show/user.getWeeklyTrackChart"/>.
  /// <seealso href="https://www.last.fm/api/show/user.getWeeklyAlbumChart"/>.
  public async Task<ApiResult<IReadOnlyList<T>>> GetWeeklyChartAsync<T>(string username, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null, CancellationToken ct = default) where T : IUserChartable
  {
    var parameters = new Dictionary<string, string>
    {
      ["user"] = username
    };

    if (fromDate != null)
      parameters["from"] = fromDate.Value.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture);
    if (toDate != null)
      parameters["to"] = toDate.Value.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture);

    var iChartablePropertyName = GetTypeJsonPropertyName(typeof(T));
    var result = await _invoker.SendAsync($"user.getWeekly{iChartablePropertyName}Chart", parameters, false, ct);
    if (!result.IsSuccess || result.Data == null)
      return ApiResult<IReadOnlyList<T>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

    try
    {
      var chartArray = result.Data.RootElement.GetProperty($"weekly{iChartablePropertyName.ToLower(System.Globalization.CultureInfo.CurrentCulture)}chart")
                                              .TryGetProperty(iChartablePropertyName.ToLower(System.Globalization.CultureInfo.CurrentCulture), out var ta) ? ta : default;
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
      return ApiResult<IReadOnlyList<T>>.Failure(null, result.HttpStatus, $"Failed to parse weekly {iChartablePropertyName} chart: " + ex.Message);
    }
  }

  internal static T ITagableFromJson<T>(JsonElement root) where T : IUserTagable
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

  internal static T IChartableFromJson<T>(JsonElement root) where T : IUserChartable
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