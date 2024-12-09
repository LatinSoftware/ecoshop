namespace OrderService.Shared
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public ICollection<ApiError> Errors { get; set; } = [];

    }

    public class ApiResponse
    {
        public int Code { get; set; }
        public string? Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public ICollection<ApiError> Errors { get; set; } = [];
    }

    public class ApiError
    {
        public string Code { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }

    public class PaginationMetadata
    {
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public string? NextPageUrl { get; set; } = string.Empty;
        public string? PreviousPageUrl { get; set; } = string.Empty;

        public PaginationMetadata(int currentPage, int totalItems, int pageSize, Func<int, string> generatePageUrl)
        {
            CurrentPage = currentPage;
            TotalItems = totalItems;
            PageSize = pageSize;

            NextPageUrl = HasNextPage ? generatePageUrl(CurrentPage + 1) : null;
            PreviousPageUrl = HasPreviousPage ? generatePageUrl(CurrentPage - 1) : null;
        }
    }
}
