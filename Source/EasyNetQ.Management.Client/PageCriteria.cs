namespace EasyNetQ.Management.Client;

public record PageCriteria(int Page, int PageSize, string? Name = null, bool UseRegex = false)
{
    public readonly IEnumerable<KeyValuePair<string, string>> QueryParameters =
        [
            new KeyValuePair<string, string>("page", Page.ToString()),
            new KeyValuePair<string, string>("page_size", PageSize.ToString()),
            new KeyValuePair<string, string>("name", Name ?? ""),
            new KeyValuePair<string, string>("use_regex", UseRegex.ToString().ToLower()),
            new KeyValuePair<string, string>("pagination", "true")
        ];
}
