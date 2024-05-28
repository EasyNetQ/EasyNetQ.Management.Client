using System.Net;
using System.Threading;

namespace EasyNetQ.Management.Client.Internals;

internal static class HttpResponseMessageExtensions
{
    public static async Task EnsureExpectedStatusCodeAsync(this HttpResponseMessage response, Func<HttpStatusCode, bool> isExpected, CancellationToken cancellationToken = default)
    {
        if (!isExpected(response.StatusCode))
            throw await UnexpectedHttpStatusCodeException.FromHttpResponseMessageAsync(response, cancellationToken).ConfigureAwait(false);
    }
}
