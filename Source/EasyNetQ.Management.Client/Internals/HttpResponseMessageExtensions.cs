using System.Net;

namespace EasyNetQ.Management.Client.Internals;

internal static class HttpResponseMessageExtensions
{
    public static void EnsureExpectedStatusCode(this HttpResponseMessage response, Func<HttpStatusCode, bool> isExpected)
    {
        if (!isExpected(response.StatusCode))
            throw new UnexpectedHttpStatusCodeException(response);
    }
}
