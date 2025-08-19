using System.Text.Json;

namespace Shoegaze.LastFM
{
  /// <summary>
  /// A result that contains the items of a single page request.
  /// </summary>
  /// <typeparam name="T">Type of the result items.</typeparam>
  public class PagedResult<T>
  {
    /// <summary>
    /// The result items.
    /// </summary>
    public IReadOnlyList<T> Items { get; private set; } = [];

    /// <summary>
    /// The page that has been fetched.
    /// </summary>
    public int Page { get; private set; }

    /// <summary>
    /// Total amount of pages.
    /// </summary>
    public int TotalPages { get; private set; }

    /// <summary>
    /// Total amount of items.
    /// </summary>
    public int TotalItems { get; private set; }

    /// <summary>
    /// Amount of items per page.
    /// </summary>
    public int PerPage { get; private set; }

    internal static PagedResult<T> FromJson(JsonElement element, IReadOnlyList<T> items)
    {
      int parsedPage, totalPages, totalItems, perPage;

      if (element.TryGetProperty("opensearch:Query", out var _)) // opensearch format
      {
        totalItems = JsonHelper.ParseNumber<int>(element.GetProperty("opensearch:totalResults"));
        perPage = JsonHelper.ParseNumber<int>(element.GetProperty("opensearch:itemsPerPage"));
        totalPages = (int)Math.Ceiling((double)totalItems / perPage);
        parsedPage = JsonHelper.ParseNumber<int>(element.GetProperty("opensearch:Query").GetProperty("startPage"));
      }
      else
      {
        var attr = element.GetProperty("@attr");
        if (attr.TryGetProperty("offset", out var offsetProp)) // assume offset format
        {
          totalItems = JsonHelper.ParseNumber<int>(attr.GetProperty("total"));
          perPage = JsonHelper.ParseNumber<int>(attr.GetProperty("num_res"));
          parsedPage = JsonHelper.ParseNumber<int>(offsetProp) / perPage;
          if (parsedPage == 0)
            parsedPage = 1;
          totalPages = (int)Math.Ceiling((double)totalItems / perPage);
        }
        else // assume default page format
        {
          parsedPage = int.TryParse(attr.GetProperty("page").GetString(), out var p) ? p : 1;
          totalPages = int.TryParse(attr.GetProperty("totalPages").GetString(), out var tp) ? tp : 1;
          totalItems = int.TryParse(attr.GetProperty("total").GetString(), out var t) ? t : items.Count;
          perPage = int.TryParse(attr.GetProperty("perPage").GetString(), out var pp) ? pp : items.Count;
        }
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