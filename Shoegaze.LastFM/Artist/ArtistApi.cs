using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Tag;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Artist
{
  public class ArtistApi : IArtistApi
  {
    private readonly ILastfmApiInvoker _invoker;

    internal ArtistApi(ILastfmApiInvoker invoker) => _invoker = invoker;

    public async Task<ApiResult<ArtistInfo>> GetInfoByNameAsync(string artistName, string? username = null, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["artist"] = artistName,
      };

      return await GetInfoAsync(parameters, username, autoCorrect, ct);
    }

    public async Task<ApiResult<ArtistInfo>> GetInfoByMbidAsync(string mbid, string? username = null, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["mbid"] = mbid,
      };

      return await GetInfoAsync(parameters, username, autoCorrect, ct);
    }

    private async Task<ApiResult<ArtistInfo>> GetInfoAsync(Dictionary<string, string> parameters, string? username, bool autoCorrect = true, CancellationToken ct = default)
    {
      if (username != null)
        parameters["username"] = username;
      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      var result = await _invoker.SendAsync("artist.getInfo", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<ArtistInfo>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var artistInfo = ArtistInfo.FromJson(result.Data.RootElement.GetProperty("artist"));
        if (username == null)
          artistInfo.UserPlayCount = null; // usercount may have the same json property name as plays; if user is null userplaycount should be null

        return ApiResult<ArtistInfo>.Success(artistInfo);
      }
      catch (Exception ex)
      {
        return ApiResult<ArtistInfo>.Failure(null, result.HttpStatus, "Failed to parse artist info: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<ArtistInfo>>> GetSimilarByNameAsync(string artistName, bool autoCorrect = true, int? limit = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, null);
      parameters.Add("artist", artistName);

      return await GetSimilarAsync(parameters, autoCorrect, ct);
    }

    public async Task<ApiResult<IReadOnlyList<ArtistInfo>>> GetSimilarByMbidAsync(string mbid, bool autoCorrect = true, int? limit = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, null);
      parameters.Add("mbid", mbid);

      return await GetSimilarAsync(parameters, autoCorrect, ct);
    }

    private async Task<ApiResult<IReadOnlyList<ArtistInfo>>> GetSimilarAsync(Dictionary<string, string> parameters, bool autoCorrect = true, CancellationToken ct = default)
    {
      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      var result = await _invoker.SendAsync("artist.getSimilar", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<IReadOnlyList<ArtistInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var artistArray = result.Data.RootElement.GetProperty("similarartists").TryGetProperty("artist", out var ta) ? ta : default;
        var artists = JsonHelper.MakeListFromJsonArray(artistArray, ArtistInfo.FromJson);

        return ApiResult<IReadOnlyList<ArtistInfo>>.Success(artists);
      }
      catch (Exception ex)
      {
        return ApiResult<IReadOnlyList<ArtistInfo>>.Failure(null, result.HttpStatus, "Failed to parse artist info: " + ex.Message);
      }
    }

    public async Task<ApiResult<ArtistInfo>> GetCorrectionAsync(string artistName, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["artist"] = artistName,
      };

      var result = await _invoker.SendAsync("artist.getCorrection", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<ArtistInfo>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var artistInfo = ArtistInfo.FromJson(result.Data.RootElement.GetProperty("corrections").GetProperty("correction").GetProperty("artist"));
        return ApiResult<ArtistInfo>.Success(artistInfo);
      }
      catch (Exception ex)
      {
        return ApiResult<ArtistInfo>.Failure(null, result.HttpStatus, "Failed to parse artist info: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByNameAsync(string artistName, string? username = null, bool autocorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["artist"] = artistName,
      };

      return await GetTagsAsync(parameters, username, autocorrect, ct);
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByMbidAsync(string mbid, string? username = null, bool autocorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["mbid"] = mbid,
      };

      return await GetTagsAsync(parameters, username, autocorrect, ct);
    }

    private async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsAsync(Dictionary<string, string> parameters, string? username = null, bool autoCorrect = true, CancellationToken ct = default)
    {
      if (username != null)
        parameters.Add("user", username);
      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      var result = await _invoker.SendAsync("artist.getTags", parameters, username == null, ct);
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

    public async Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsByNameAsync(string artistName, bool autoCorrect = true, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("artist", artistName);

      return await GetTopAlbumsAsync(parameters, autoCorrect, ct);
    }

    public async Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsByMbidAsync(string mbid, bool autoCorrect = true, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("mbid", mbid);

      return await GetTopAlbumsAsync(parameters, autoCorrect, ct);
    }

    private async Task<ApiResult<PagedResult<AlbumInfo>>> GetTopAlbumsAsync(Dictionary<string, string> parameters, bool autoCorrect = true, CancellationToken ct = default)
    {
      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      var result = await _invoker.SendAsync("artist.getTopAlbums", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<AlbumInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var albumsProperty = result.Data.RootElement.GetProperty("topalbums");
        var albumArray = albumsProperty.TryGetProperty("album", out var ta) ? ta : default;
        var albums = JsonHelper.MakeListFromJsonArray(albumArray, AlbumInfo.FromJson);

        foreach (var album in albums)
          album.UserPlayCount = null; // same property name as PlayCount

        return ApiResult<PagedResult<AlbumInfo>>.Success(PagedResult<AlbumInfo>.FromJson(albumsProperty, albums));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<AlbumInfo>>.Failure(null, result.HttpStatus, "Failed to parse albums: " + ex.Message);
      }
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByNameAsync(string artistName, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["artist"] = artistName
      };

      return await GetTopTags(parameters, autoCorrect, ct);
    }

    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByMbidAsync(string mbid, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["mbid"] = mbid
      };

      return await GetTopTags(parameters, autoCorrect, ct);
    }

    private async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTags(Dictionary<string, string> parameters, bool autoCorrect = true, CancellationToken ct = default)
    {
      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      var result = await _invoker.SendAsync("artist.getTopTags", parameters, false, ct);
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
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(null, result.HttpStatus, "Failed to parse tag info: " + ex.Message);
      }
    }

    public async Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksByNameAsync(string artistName, bool autoCorrect = true, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("artist", artistName);

      return await GetTopTracksAsync(parameters, autoCorrect, ct);
    }

    public async Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksByMbidAsync(string mbid, bool autoCorrect = true, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("mbid", mbid);

      return await GetTopTracksAsync(parameters, autoCorrect, ct);
    }

    private async Task<ApiResult<PagedResult<TrackInfo>>> GetTopTracksAsync(Dictionary<string, string> parameters, bool autoCorrect = true, CancellationToken ct = default)
    {
      ParameterHelper.AddAutoCorrectParameter(parameters, autoCorrect);

      var result = await _invoker.SendAsync("artist.getTopTracks", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<TrackInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var tracksProperty = result.Data.RootElement.GetProperty("toptracks");
        var trackArray = tracksProperty.TryGetProperty("track", out var ta) ? ta : default;
        var tracks = JsonHelper.MakeListFromJsonArray(trackArray, TrackInfo.FromJson);

        foreach (var track in tracks)
          track.UserPlayCount = null; // same property as playcount

        return ApiResult<PagedResult<TrackInfo>>.Success(PagedResult<TrackInfo>.FromJson(tracksProperty, tracks));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<TrackInfo>>.Failure(null, result.HttpStatus, "Failed to parse tracks: " + ex.Message);
      }
    }

    public async Task<ApiResult<PagedResult<ArtistInfo>>> SearchAsync(string artistName, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("artist", artistName);

      var result = await _invoker.SendAsync("artist.search", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<ArtistInfo>>.Failure(result.Status, result.HttpStatus, result.ErrorMessage);

      try
      {
        var resultsProperty = result.Data.RootElement.GetProperty("results");
        var trackMatchesProperty = resultsProperty.GetProperty("artistmatches");
        var trackArray = trackMatchesProperty.TryGetProperty("artist", out var ta) ? ta : default;
        var tracks = JsonHelper.MakeListFromJsonArray(trackArray, ArtistInfo.FromJson);

        return ApiResult<PagedResult<ArtistInfo>>.Success(PagedResult<ArtistInfo>.FromJson(resultsProperty, tracks));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<ArtistInfo>>.Failure(null, result.HttpStatus, "Failed to parse artists: " + ex.Message);
      }
    }
  }
}