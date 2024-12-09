using Microsoft.EntityFrameworkCore;

namespace ProductService.Shared;

public sealed class PaginationResponse<T> : ApiResponse<IEnumerable<T>>
{
    public PaginationMetadata Metadata { get; set; }

    public PaginationResponse(IEnumerable<T> data, PaginationMetadata metadata)
    {
        Data = data;
        Metadata = metadata;
        
    }

    public static async Task<PaginationResponse<T>> CreateAsync(IQueryable<T> query, int page, int pageSize, Func<int, string> generatePageUrl)
    {
        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var metadata = new PaginationMetadata(
            currentPage: page,
            totalItems: totalCount,
            pageSize: pageSize,
            generatePageUrl: generatePageUrl
        );

        return new PaginationResponse<T>(items, metadata);
    }
}
