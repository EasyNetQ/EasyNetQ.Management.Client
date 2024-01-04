using System.Net;
using System.Runtime.Serialization;

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

    private const string NoRequest = "<null>";

    public HttpStatusCode StatusCode { get; private init; }
    public int StatusCodeNumber => (int)StatusCode;

    public UnexpectedHttpStatusCodeException()
    {
    }

    public UnexpectedHttpStatusCodeException(HttpStatusCode statusCode) :
        base($"Unexpected Status Code: {(int)statusCode} {statusCode}")
    {
        StatusCode = statusCode;
    }

    public UnexpectedHttpStatusCodeException(HttpResponseMessage response) :
        base($"Unexpected Status Code: {(int)response.StatusCode} {response.StatusCode} from request: {response.RequestMessage?.ToString() ?? NoRequest}")
    {
        StatusCode = response.StatusCode;
    }

    public UnexpectedHttpStatusCodeException(string message) : base(message)
    {
    }

    public UnexpectedHttpStatusCodeException(string message, Exception inner) : base(message, inner)
    {
    }

    protected UnexpectedHttpStatusCodeException(
        SerializationInfo info,
        StreamingContext context
    ) : base(info, context)
    {
    }
}
