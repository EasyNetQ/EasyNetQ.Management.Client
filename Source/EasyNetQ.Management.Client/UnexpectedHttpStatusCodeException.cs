using System;
using System.Net;

namespace EasyNetQ.Management.Client
{
#if NETFX
    [Serializable]
#endif
    public class UnexpectedHttpStatusCodeException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public HttpStatusCode StatusCode { get; private set; }
        public int StatusCodeNumber { get; private set; }

        public UnexpectedHttpStatusCodeException()
        {
        }

        public UnexpectedHttpStatusCodeException(HttpStatusCode statusCode) :
            base($"Unexpected Status Code: {(int) statusCode} {statusCode}")
        {
            StatusCode = statusCode;
            StatusCodeNumber = (int) statusCode;
        }

        public UnexpectedHttpStatusCodeException(string message) : base(message)
        {
        }

        public UnexpectedHttpStatusCodeException(string message, Exception inner) : base(message, inner)
        {
        }
#if NETFX
        protected UnexpectedHttpStatusCodeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}