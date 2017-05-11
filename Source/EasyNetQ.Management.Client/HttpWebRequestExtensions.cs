using System.Net.Http;
using System.Threading.Tasks;

namespace EasyNetQ.Management.Client
{
    public static class HttpWebRequestExtensions
    {
        public static HttpResponseMessage GetHttpResponse(this HttpClient client, HttpRequestMessage request)
        {
            return GetHttpResponseAsync(client, request).Result;
        }

        /// <summary>
        /// https://blogs.msdn.microsoft.com/pfxteam/2012/04/13/should-i-expose-synchronous-wrappers-for-asynchronous-methods/
        /// </summary>
        public static async Task<HttpResponseMessage> GetHttpResponseAsync(this HttpClient client, HttpRequestMessage request)
        {
            return await client.SendAsync(request).ConfigureAwait(false);
        }
    }
}