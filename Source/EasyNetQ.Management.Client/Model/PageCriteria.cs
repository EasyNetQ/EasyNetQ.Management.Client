namespace EasyNetQ.Management.Client.Model;

public class PageCriteria
{
    public int Page { get; }
    public int PageSize { get; }
    public string? Name { get; }
    public bool UseRegex { get; }

    public PageCriteria(int page, int pageSize, string? name = null, bool useRegex = false)
    {
        Page = page;
        PageSize = pageSize;
        Name = name;
        UseRegex = useRegex;
    }

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
