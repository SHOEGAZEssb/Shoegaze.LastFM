using Shoegaze.LastFM.User;
using System.Diagnostics;
using System.Text.Json;

namespace Shoegaze.LastFM
{
  public class PagedResult<T>
  {
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int TotalPages { get; init; }
    public int TotalItems { get; init; }
    public int PerPage { get; init; }

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