namespace EasyNetQ.Management.Client.Model;

public class PageResult<T>
{
    public int FilteredCount { get; }
    public int ItemCount { get; }
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageCount { get; }
    public int PageSize { get; }
    public int TotalCount { get; }

    public PageResult(int filteredCount, int itemCount, T[] items, int page, int pageCount, int pageSize, int totalCount)
    {
        FilteredCount = filteredCount;
        ItemCount = itemCount;
        Items = items;
        Page = page;
        PageCount = pageCount;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
