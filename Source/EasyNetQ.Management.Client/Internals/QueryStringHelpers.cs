using System.Text;

namespace EasyNetQ.Management.Client.Internals;

internal static class QueryStringHelpers
{
    public static string AddQueryString(string uri, IEnumerable<KeyValuePair<string, string>>? queryParameters)
    {
        if (queryParameters == null) return uri;

        var queryParameterSeparator = uri.Contains('?') ? '&' : '?';
        var sb = new StringBuilder(uri);
        foreach (var parameter in queryParameters)
        {
            sb.Append(queryParameterSeparator);
            sb.Append(Uri.EscapeDataString(parameter.Key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(parameter.Value));
            queryParameterSeparator = '&';
        }
        return sb.ToString();
    }
}
