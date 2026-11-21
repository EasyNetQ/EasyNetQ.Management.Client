using System.Net;
using System.Text;
using Refit;

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
        string content;
        try
        {
#if NET
            content = $"'{await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)}'";
#else
            content = $"'{await response.Content.ReadAsStringAsync().ConfigureAwait(false)}'";
#endif
        }
        catch
        {
            content = "<not a string>";
        }
        return Create(response.StatusCode, content, response.RequestMessage);
    }

    internal static UnexpectedHttpStatusCodeException From(HttpStatusCode statusCode, IApiResponse response) =>
        Create(statusCode, response.Error is ApiException { Content: { } content } ? $"'{content}'" : "<null>", response.RequestMessage);

    private static UnexpectedHttpStatusCodeException Create(HttpStatusCode statusCode, string content, HttpRequestMessage? request)
    {
        var sb = new StringBuilder("Unexpected response: StatusCode: ");
        sb.Append((int)statusCode);
        sb.Append(' ');
        sb.Append(statusCode);
        sb.Append(", Content: ");
        sb.Append(content);
        sb.Append(" from request: ");
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

        return new UnexpectedHttpStatusCodeException(sb.ToString(), statusCode);
    }
}
