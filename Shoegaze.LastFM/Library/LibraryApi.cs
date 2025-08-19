using Shoegaze.LastFM.Artist;

namespace Shoegaze.LastFM.Library
{
  /// <summary>
  /// Access to library-related api endpoints.
  /// </summary>
  public class LibraryApi : ILibraryApi
  {
    private readonly ILastfmApiInvoker _invoker;

    internal LibraryApi(ILastfmApiInvoker invoker)
    {
      _invoker = invoker;
    }

    /// <summary>
    /// Get a list of all the artists in a users library.
    /// </summary>
    /// <param name="username">Name of the user.</param>
    /// <param name="limit">Number of results per page (defaults to 50).</param>
    /// <param name="page">Page number (defaults to first page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result that contains a list of artists in the users library, or error information.</returns>
    public async Task<ApiResult<PagedResult<ArtistInfo>>> GetArtistsAsync(string username, int? limit = null, int? page = null, CancellationToken ct = default)
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit, page);
      parameters.Add("user", username);

      var result = await _invoker.SendAsync("library.getArtists", parameters, false, ct);
      if (!result.IsSuccess || result.Data == null)
        return ApiResult<PagedResult<ArtistInfo>>.Failure(result.LastFmStatus, result.HttpStatus, result.ErrorMessage);

      try
      {
        var artistsProp = result.Data.RootElement.GetProperty("artists");
        var artistArray = artistsProp.TryGetProperty("artist", out var ta) ? ta : default;
        var artists = JsonHelper.MakeListFromJsonArray(artistArray, ArtistInfo.FromJson);

        foreach (var artist in artists)
          artist.PlayCount = null;

        return ApiResult<PagedResult<ArtistInfo>>.Success(PagedResult<ArtistInfo>.FromJson(artistsProp, artists));
      }
      catch (Exception ex)
      {
        return ApiResult<PagedResult<ArtistInfo>>.Failure(null, result.HttpStatus, $"Failed to parse top artists: {ex.Message}");
      }
    }
  }
}