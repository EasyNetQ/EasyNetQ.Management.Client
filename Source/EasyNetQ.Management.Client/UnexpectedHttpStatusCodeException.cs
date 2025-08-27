using System.Net;
using System.Text;

namespace EasyNetQ.Management.Client;

[Serializable]
public class UnexpectedHttpStatusCodeException : Exception
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public HttpStatusCode StatusCode { get; private init; }
    public int StatusCodeNumber => (int)StatusCode;

    public UnexpectedHttpStatusCodeException()
    {
    }

    public UnexpectedHttpStatusCodeException(string message) : base(message)
    {
    }

    public UnexpectedHttpStatusCodeException(string message, Exception inner) : base(message, inner)
    {
    }

    protected UnexpectedHttpStatusCodeException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public static async Task<UnexpectedHttpStatusCodeException> FromHttpResponseMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder("Unexpected response: StatusCode: ");
        sb.Append((int)response.StatusCode);
        sb.Append(" ");
        sb.Append(response.StatusCode);
        sb.Append(", Content: ");
        if (response.Content != null)
        {
            try
            {
#if NET5_0_OR_GREATER
                var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
                sb.Append('\'');
                sb.Append(content);
                sb.Append('\'');
            }
            catch
            {
                sb.Append("<not a string>");
            }
        }
        else
        {
            sb.Append("<null>");
        }
        sb.Append(" from request: ");
        var request = response.RequestMessage;
        if (request != null)
        {
            sb.Append("Method: ");
            sb.Append(request.Method.ToString());

            sb.Append(", RequestUri: '");
            sb.Append(request.RequestUri == null ? "<null>" : request.RequestUri.ToString());

            sb.Append("', Version: ");
            sb.Append(request.Version.ToString());

            sb.Append(", Content: ");
            sb.Append(request.Content == null ? "<null>" : request.Content.GetType().ToString());
        }
        else
        {
            sb.Append("<null>");
        }

        return new UnexpectedHttpStatusCodeException(sb.ToString(), response.StatusCode);
    }
}
