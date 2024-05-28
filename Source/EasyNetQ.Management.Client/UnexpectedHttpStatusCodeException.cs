using System.Net;
using System.Runtime.Serialization;
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

    public UnexpectedHttpStatusCodeException(HttpResponseMessage response) :
        base(BuildMessage(response))
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

    private static string BuildMessage(HttpResponseMessage response)
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
                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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
        if (response.RequestMessage != null)
        {
            sb.Append(response.RequestMessage);
        }
        else
        {
            sb.Append("<null>");
        }

        return sb.ToString();
    }
}
