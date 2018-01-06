using System;

namespace EasyNetQ.Management.Client
{
#if NETFX
    [Serializable]
#endif
    public class EasyNetQManagementException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public EasyNetQManagementException()
        {
        }

        public EasyNetQManagementException(string format, params object[] args) : base(string.Format(format, args))
        {
        }

        public EasyNetQManagementException(string message) : base(message)
        {
        }

        public EasyNetQManagementException(string message, Exception inner) : base(message, inner)
        {
        }
#if NETFX
        protected EasyNetQManagementException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}