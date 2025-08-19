using Shoegaze.LastFM.Tag;

namespace Shoegaze.LastFM.Album
{
  /// <summary>
  /// Access to album-related API endpoints.
  /// </summary>
  public class AlbumApi : IAlbumApi
  {
    private readonly ILastfmApiInvoker _invoker;

    internal AlbumApi(ILastfmApiInvoker invoker) => _invoker = invoker;

    /// <summary>
    /// Get the metadata for an album.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="username">The username for the context of the request.
    /// If supplied, the users playcount for this album is included
    /// in the response.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names,
    /// returning the correct version instead</param>
    /// <param name="language">The language to return the biography in, expressed as an ISO 639 alpha-2 code.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result object that contains the album metadata, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.getInfo"/>.
    public async Task<ApiResult<AlbumInfo>> GetInfoByNameAsync(string albumName, string artistName, string? username = null, bool autoCorrect = true, string? language = null, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["album"] = albumName,
        ["artist"] = artistName
      };

      return await GetInfoAsync(parameters, username, autoCorrect, language, ct);
    }

    /// <summary>
    /// Get the metadata for an album.
    /// </summary>
    /// <param name="mbid">Musicbrainz ID of the album.</param>
    /// <param name="username">The username for the context of the request.
    /// If supplied, the users playcount for this album is included
    /// in the response.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names,
    /// returning the correct version instead.</param>
    /// <param name="language">The language to return the biography in, expressed as an ISO 639 alpha-2 code.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response from the last.fm api that
    /// contains the album metadata, or error information.</returns>
    /// <remarks>
    /// <see href="https://www.last.fm/api/show/album.getInfo"/>.
    /// </remarks>
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
        return ApiResult<AlbumInfo>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

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

    /// <summary>
    /// Get the tags applied by an individual user to an album.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">name of the artist.</param>
    /// <param name="username">User to look up. If null, the authenticated session username will be used.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names,
    /// returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result object that contains the tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.getTags"/>.
    public async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTagsByNameAsync(string albumName, string artistName, string? username = null, bool autoCorrect = true, CancellationToken ct = default)
    {
      var parameters = new Dictionary<string, string>
      {
        ["album"] = albumName,
        ["artist"] = artistName
      };

      return await GetTagsAsync(parameters, username, autoCorrect, ct);
    }

    /// <summary>
    /// Get the tags applied by an individual user to an album.
    /// </summary>
    /// <param name="mbid">Musibrainz ID of the album.</param>
    /// <param name="username">User to look up. If null, the authenticated session username will be used.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names,
    /// returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result object that contains the tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.getTags"/>.
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
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

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

    /// <summary>
    /// Get the top tags for an album, ordered by popularity.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="autoCorrect">Transform misspelled artist names into correct artist names,
    /// returning the correct version instead.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result object that contains the tags, or error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.getTopTags"/>.
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
    private async Task<ApiResult<IReadOnlyList<TagInfo>>> GetTopTagsByMbidAsync(string mbid, bool autoCorrect = true, CancellationToken ct = default)
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
        return ApiResult<IReadOnlyList<TagInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

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

    /// <summary>
    /// Search for an album by name.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="limit">Number of results to fetch per page. Defaults to 30.</param>
    /// <param name="page">The page number to fetch. Defaults to first page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains the found albums sorted by relevancy, or error information..</returns>
    /// <seealso href="https://www.last.fm/api/show/album.search"/>.
    public async Task<ApiResult<PagedResult<AlbumInfo>>> SearchAsync(string albumName, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("album", albumName);

      var result = await _invoker.SendAsync("album.search", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<AlbumInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var resultsProperty = result.Data.RootElement.GetProperty("results");
        var albumMatchesProperty = resultsProperty.GetProperty("albummatches");
        var albumArray = albumMatchesProperty.TryGetProperty("album", out var ta) ? ta : default;
        var albums = JsonHelper.MakeListFromJsonArray(albumArray, AlbumInfo.FromJson);

        return ApiResult<PagedResult<AlbumInfo>>.Success(PagedResult<AlbumInfo>.FromJson(resultsProperty, albums));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<AlbumInfo>>.Failure(null, result.HttpStatus, "Failed to parse albums: " + ex.Message);
      }
    }

    /// <summary>
    /// Tag an album using a user supplied tag.
    /// This method requires authentication.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tag">Tag to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.addTags"/>.
    public async Task<ApiResult> AddTagsAsync(string albumName, string artistName, string tag, CancellationToken ct = default)
    {
      return await AddTagsAsync(albumName, artistName, [tag], ct);
    }

    /// <summary>
    /// Tag an album using a list of user supplied tags.
    /// This method requires authentication.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tags">Tags to add. Maximum of 10 allowed.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.addTags"/>.
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="tags"/> has more than 10 or less than 1 tag.</exception>
    public async Task<ApiResult> AddTagsAsync(string albumName, string artistName, IEnumerable<string> tags, CancellationToken ct = default)
    {
      var tagCount = tags.Count();
      if (tagCount > 10)
        throw new ArgumentOutOfRangeException(nameof(tags), "Only maximum of 10 tags allowed");
      if (tagCount == 0)
        throw new ArgumentOutOfRangeException(nameof(tags), "No tags supplied");

      var parameters = new Dictionary<string, string>
      {
        ["album"] = albumName,
        ["artist"] = artistName,
        ["tags"] = string.Join(",", tags)
      };

      var result = await _invoker.SendAsync("album.addTags", parameters, true, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      return ApiResult.Success();
    }

    /// <summary>
    /// Remove a users tag from an album.
    /// This method requires authentication.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tag">Tag to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.removeTag"/>.
    public async Task<ApiResult> RemoveTagsAsync(string albumName, string artistName, string tag, CancellationToken ct = default)
    {
      return await RemoveTagsAsync(albumName, artistName, [tag], ct);
    }

    /// <summary>
    /// Remove a users list of tags from an album.
    /// This method requires authentication.
    /// </summary>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="tags">List of tags to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result containing error information.</returns>
    /// <seealso href="https://www.last.fm/api/show/album.removeTag"/>.
    public async Task<ApiResult> RemoveTagsAsync(string albumName, string artistName, IEnumerable<string> tags, CancellationToken ct = default)
    {
      foreach (var tag in tags)
      {
        var parameters = new Dictionary<string, string>
        {
          ["album"] = albumName,
          ["artist"] = artistName,
          ["tags"] = tag
        };

        var result = await _invoker.SendAsync("album.removeTag", parameters, true, ct);
        if (!result.IsSuccess || result.Data == null)
          return ApiResult.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);
      }

      return ApiResult.Success();
    }
  }
}