namespace JobPortal.Application.Common.Models;

/// <summary>
/// Standardized paginated list for all list queries.
/// Uses page number + page size (offset-based). Cursor-based available per query if needed.
/// </summary>
public sealed class PagedList<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public bool HasNextPage => PageNumber * PageSize < TotalCount;
    public bool HasPreviousPage => PageNumber > 1;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static PagedList<T> Create(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize) =>
        new()
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

    public static PagedList<T> Empty(int pageNumber, int pageSize) =>
        Create([], 0, pageNumber, pageSize);
}

/// <summary>
/// Standard pagination parameters for all list queries.
/// </summary>
public record PaginationParams
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public int Skip => (PageNumber - 1) * PageSize;

    public PaginationParams WithDefaults(int maxPageSize = 100) =>
        this with
        {
            PageNumber = Math.Max(1, PageNumber),
            PageSize = Math.Clamp(PageSize, 1, maxPageSize)
        };
}
