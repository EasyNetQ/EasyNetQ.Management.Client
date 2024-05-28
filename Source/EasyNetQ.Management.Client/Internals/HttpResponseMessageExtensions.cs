using System.Net;

namespace EasyNetQ.Management.Client.Internals;

internal static class HttpResponseMessageExtensions
{
    public static async Task EnsureExpectedStatusCodeAsync(this HttpResponseMessage response, Func<HttpStatusCode, bool> isExpected)
    {
        if (!isExpected(response.StatusCode))
            throw await UnexpectedHttpStatusCodeException.FromHttpResponseMessageAsync(response).ConfigureAwait(false);
    }
}
