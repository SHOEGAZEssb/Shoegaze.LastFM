namespace Shoegaze.LastFM
{
  public class PagedResult<T>
  {
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int TotalPages { get; init; }
    public int TotalItems { get; init; }
    public int PerPage { get; init; }
  }
}