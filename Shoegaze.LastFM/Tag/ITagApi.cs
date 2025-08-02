namespace Shoegaze.LastFM.Tag
{
  public interface ITagApi
  {
    Task<ApiResult<TagInfo>> GetInfoAsync(
      string name,
      CancellationToken ct = default);
  }
}