namespace Shoegaze.LastFM.Tag
{
  public interface ITagApi
  {
    Task<ApiResult<TagInfo>> GetInfoAsync(
      string name,
      CancellationToken ct = default);

    /// <summary>
    /// Get tags similar to the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Tag to get similar tags for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of similar tags.</returns>
    /// <remarks>
    /// Broken on last.fms side currently.
    /// Will only return an empty list.
    /// </remarks>
    Task<ApiResult<IReadOnlyList<TagInfo>>> GetSimilarAsync(
      string name,
      CancellationToken ct = default);
  }
}