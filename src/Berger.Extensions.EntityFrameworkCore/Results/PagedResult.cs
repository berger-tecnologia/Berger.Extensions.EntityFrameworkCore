namespace Berger.Extensions.EntityFrameworkCore;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, long TotalItems)
{
    public long TotalPages => PageSize < 1 ? 0 : (long)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
    public static PagedResult<T> Empty(int page, int pageSize) => new([], page, pageSize, 0);
}
