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
      int parsedPage, totalPages, totalItems, perPage;

      if (element.TryGetProperty("opensearch:Query", out var _)) // opensearch format
      {
        totalItems = JsonHelper.ParseNumber<int>(element.GetProperty("opensearch:totalResults"));
        perPage = JsonHelper.ParseNumber<int>(element.GetProperty("opensearch:itemsPerPage"));
        totalPages = totalItems / perPage;
        parsedPage = JsonHelper.ParseNumber<int>(element.GetProperty("opensearch:Query").GetProperty("startPage"));
      }
      else
      {
        var attr = element.GetProperty("@attr");
        parsedPage = int.TryParse(attr.GetProperty("page").GetString(), out var p) ? p : 1;
        totalPages = int.TryParse(attr.GetProperty("totalPages").GetString(), out var tp) ? tp : 1;
        totalItems = int.TryParse(attr.GetProperty("total").GetString(), out var t) ? t : items.Count;
        perPage = int.TryParse(attr.GetProperty("perPage").GetString(), out var pp) ? pp : items.Count;
      }

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