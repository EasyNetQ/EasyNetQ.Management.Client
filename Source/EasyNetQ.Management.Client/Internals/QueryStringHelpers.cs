using System.Text;

namespace EasyNetQ.Management.Client.Internals;

internal static class QueryStringHelpers
{
    public static string AddQueryString(string uri, IReadOnlyDictionary<string, string>? queryString)
    {
        if (queryString == null || queryString.Count == 0) return uri;

        var queryIndex = uri.IndexOf('?');
        var hasQuery = queryIndex != -1;
        var sb = new StringBuilder();
        sb.Append(uri);
        foreach (var parameter in queryString)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append(Uri.EscapeDataString(parameter.Key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(parameter.Value));
            hasQuery = true;
        }
        return sb.ToString();
    }
}
