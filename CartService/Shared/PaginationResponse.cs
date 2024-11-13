
namespace CartService.Shared;

public class PaginationResponse<T>
{
    public IEnumerable<T> Data { get; set; }
    public int CurrentPage { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public string? NextPageUrl { get; set; } = string.Empty;
    public string? PreviousPageUrl { get; set; } = string.Empty;

    public PaginationResponse(IEnumerable<T> data, int currentPage, int totalItems, int pageSize, Func<int, string> generatePageUrl)
    {
        Data = data;
        CurrentPage = currentPage;
        TotalItems = totalItems;
        PageSize = pageSize;

        NextPageUrl = HasNextPage ? generatePageUrl(CurrentPage + 1) : null;
        PreviousPageUrl = HasPreviousPage ? generatePageUrl(CurrentPage - 1) : null;
    }

    public static async Task<PaginationResponse<T>> CreateAsync(IQueryable<T> query, int page, int pageSize, Func<int, string> generatePageUrl)
    {
        var totalCount = query.Count();
        var items =  query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        await Task.CompletedTask;

        return new(items, page, totalCount, pageSize, generatePageUrl);
    }
}
