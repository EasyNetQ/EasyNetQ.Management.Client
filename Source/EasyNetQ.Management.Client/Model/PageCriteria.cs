using System.Collections.Generic;

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

    public IReadOnlyDictionary<string, string> ToQueryParameters(IReadOnlyDictionary<string, string> queryParameters)
    {
        var result = ToQueryParameters() as Dictionary<string, string>;
        foreach (var queryParameter in queryParameters)
        {
            result!.Add(queryParameter.Key, queryParameter.Value);
        }
        return result!;
    }
}
