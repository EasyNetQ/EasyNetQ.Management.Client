namespace EasyNetQ.Management.Client.Model;

public record PageCriteria(int Page, int PageSize, string? Name = null, bool UseRegex = false)
{
    public IReadOnlyDictionary<string, string> ToQueryParameters()
    {
        return new Dictionary<string, string>
        {
            { "page", Page.ToString() },
            { "page_size", PageSize.ToString() },
            { "name", Name ?? "" },
            { "use_regex", UseRegex.ToString().ToLower() },
            { "pagination", "true" }
        };
    }
}
