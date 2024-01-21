using System.Text.RegularExpressions;

namespace EasyNetQ.Management.Client;

internal static class LegacyEndpointBuilder
{
    private static readonly Regex UrlRegex = new(@"^(http|https):\/\/\[?.+\w\]?$", RegexOptions.Compiled | RegexOptions.Singleline);

    public static Uri Build(string hostUrl, int portNumber, bool ssl)
    {
        if (string.IsNullOrEmpty(hostUrl)) throw new ArgumentException("hostUrl is null or empty");

        if (hostUrl.StartsWith("https://")) ssl = true;

        if (ssl)
        {
            if (hostUrl.StartsWith("http://")) throw new ArgumentException("hostUrl is illegal");
            hostUrl = hostUrl.StartsWith("https://") ? hostUrl : "https://" + hostUrl;
        }
        else
        {
            if (hostUrl.StartsWith("https://")) throw new ArgumentException("hostUrl is illegal");
            hostUrl = hostUrl.StartsWith("http://") ? hostUrl : "http://" + hostUrl;
        }

        if (!UrlRegex.IsMatch(hostUrl)) throw new ArgumentException("hostUrl is illegal");

        return Uri.TryCreate(portNumber != 443 ? $"{hostUrl}:{portNumber}" : hostUrl, UriKind.Absolute, out var uri)
            ? uri
            : throw new ArgumentException("hostUrl is illegal");
    }
}
