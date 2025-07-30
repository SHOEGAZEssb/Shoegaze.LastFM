using System.Text.Json;

namespace Shoegaze.LastFM
{
  public class PagedResult<T>
  {
    public IReadOnlyList<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int PerPage { get; set; }

    internal static PagedResult<T> FromJson(JsonElement element, List<T> items)
    {
      var attr = element.GetProperty("@attr");

      var parsedPage = int.TryParse(attr.GetProperty("page").GetString(), out var p) ? p : 1;
      var totalPages = int.TryParse(attr.GetProperty("totalPages").GetString(), out var tp) ? tp : 1;
      var totalItems = int.TryParse(attr.GetProperty("total").GetString(), out var t) ? t : items.Count;
      var perPage = int.TryParse(attr.GetProperty("perPage").GetString(), out var pp) ? pp : items.Count;

      var paged = new PagedResult<T>
      {
        Items = items,
        Page = parsedPage,
        TotalPages = totalPages,
        TotalItems = totalItems,
        PerPage = perPage
      };

      return paged;
    }
  }
}