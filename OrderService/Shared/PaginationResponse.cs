namespace OrderService.Shared;

public sealed class PaginationResponse<T> : ApiResponse<IEnumerable<T>>
{
    public PaginationMetadata Metadata { get; set; }

    public PaginationResponse(IEnumerable<T> data, PaginationMetadata metadata)
    {
        Data = data;
        Metadata = metadata;

    }

    public static Task<PaginationResponse<T>> CreateAsync(IQueryable<T> query, int page, int pageSize, Func<int, string> generatePageUrl)
    {
        var totalCount = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var metadata = new PaginationMetadata(
            currentPage: page,
            totalItems: totalCount,
            pageSize: pageSize,
            generatePageUrl: generatePageUrl
        );

        return Task.FromResult(new PaginationResponse<T>(items, metadata));
    }
}
