namespace EasyNetQ.Management.Client.Model;

public record PageResult<T>(
    int FilteredCount,
    int ItemCount,
    IReadOnlyList<T> Items,
    int Page,
    int PageCount,
    int PageSize,
    int TotalCount
);
