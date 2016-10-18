using System.Net;
using System.Threading.Tasks;

namespace EasyNetQ.Management.Client
{
    public static class HttpWebRequestExtensions
    {
        public static HttpWebResponse GetHttpResponse(this HttpWebRequest request)
        {
            return GetHttpResponseAsync(request).Result;
        }

        /// <summary>
        /// https://blogs.msdn.microsoft.com/pfxteam/2012/04/13/should-i-expose-synchronous-wrappers-for-asynchronous-methods/
        /// </summary>
        public static async Task<HttpWebResponse> GetHttpResponseAsync(this HttpWebRequest request)
        {
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);
            }
            catch (WebException exception)
            {
                if (exception.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse) exception.Response;
                }
                else
                {
                    throw;
                }
            }

            return response;
        }
    }
}