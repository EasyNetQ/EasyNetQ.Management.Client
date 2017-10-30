using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.Management.Client.Model;
using EasyNetQ.Management.Client.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EasyNetQ.Management.Client
{
    public class ManagementClient : IManagementClient
    {
        private static Task CompletedTask { get; } = Task.FromResult<object>(null);

        private static readonly MediaTypeWithQualityHeaderValue JsonMediaTypeHeaderValue =
            new MediaTypeWithQualityHeaderValue("application/json");

        public static readonly JsonSerializerSettings Settings;

        private readonly Action<HttpRequestMessage> configureRequest;
        private readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(20);
        private readonly HttpClient httpClient;

        private readonly Regex urlRegex =
            new Regex(@"^(http|https):\/\/.+\w$", RegexOptions.Compiled | RegexOptions.Singleline);

        static ManagementClient()
        {
            Settings = new JsonSerializerSettings
            {
                ContractResolver = new RabbitContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            Settings.Converters.Add(new PropertyConverter());
            Settings.Converters.Add(new MessageStatsOrEmptyArrayConverter());
            Settings.Converters.Add(new QueueTotalsOrEmptyArrayConverter());
            Settings.Converters.Add(new StringEnumConverter {CamelCaseText = true});
            Settings.Converters.Add(new HaParamsConverter());
        }

        public string HostUrl { get; }

        public string Username { get; }

        public int PortNumber { get; }

        public ManagementClient(
            string hostUrl,
            string username,
            string password,
            int portNumber = 15672,
            TimeSpan? timeout = null,
            Action<HttpRequestMessage> configureRequest = null,
            bool ssl = false)
        {
            if (string.IsNullOrEmpty(hostUrl))
            {
                throw new ArgumentException("hostUrl is null or empty");
            }

            if (hostUrl.StartsWith("https://"))
                ssl = true;

            if (ssl)
            {
                if (hostUrl.Contains("http://"))
                    throw new ArgumentException("hostUrl is illegal");
                hostUrl = hostUrl.Contains("https://") ? hostUrl : "https://" + hostUrl;
            }
            else
            {
                if (hostUrl.Contains("https://"))
                    throw new ArgumentException("hostUrl is illegal");
                hostUrl = hostUrl.Contains("http://") ? hostUrl : "http://" + hostUrl;
            }
            if (!urlRegex.IsMatch(hostUrl) || !Uri.TryCreate(hostUrl, UriKind.Absolute, out var urlUri))
            {
                throw new ArgumentException("hostUrl is illegal");
            }
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username is null or empty");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("password is null or empty");
            }
            if (configureRequest == null)
            {
                configureRequest = x => { };
            }
            HostUrl = hostUrl;
            Username = username;
            PortNumber = portNumber;
            this.configureRequest = configureRequest;


            httpClient = new HttpClient(new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password)
            })
            {
                Timeout = timeout ?? defaultTimeout
            };

            //default WebRequest.KeepAlive to false to resolve spurious 'the request was aborted: the request was canceled' exceptions
            httpClient.DefaultRequestHeaders.Add("Connection", "close");
        }

        public Task<Overview> GetOverviewAsync(GetLengthsCriteria lengthsCriteria = null,
            GetRatesCriteria ratesCriteria = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<Overview>("overview", cancellationToken, lengthsCriteria, ratesCriteria);
        }

        public Task<IEnumerable<Node>> GetNodesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<IEnumerable<Node>>("nodes", cancellationToken);
        }

        public Task<Definitions> GetDefinitionsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<Definitions>("definitions", cancellationToken);
        }

        public Task<IEnumerable<Connection>> GetConnectionsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<IEnumerable<Connection>>("connections", cancellationToken);
        }

        public async Task CloseConnectionAsync(Connection connection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(connection, nameof(connection));
            await DeleteAsync($"connections/{connection.Name}", cancellationToken)
                .ConfigureAwait(false);
        }

        public Task<IEnumerable<Channel>> GetChannelsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<IEnumerable<Channel>>("channels", cancellationToken);
        }

        public Task<Channel> GetChannelAsync(string channelName, GetRatesCriteria ratesCriteria = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(channelName, nameof(channelName));

            return GetAsync<Channel>($"channels/{channelName}", cancellationToken, ratesCriteria);
        }

        public Task<IEnumerable<Exchange>> GetExchangesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<IEnumerable<Exchange>>("exchanges", cancellationToken);
        }

        public Task<Exchange> GetExchangeAsync(string exchangeName, Vhost vhost, GetRatesCriteria ratesCriteria = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(exchangeName, nameof(exchangeName));
            Ensure.ArgumentNotNull(vhost, nameof(vhost));

            return GetAsync<Exchange>($"exchanges/{SanitiseVhostName(vhost.Name)}/{exchangeName}", cancellationToken,
                ratesCriteria);
        }

        public Task<Queue> GetQueueAsync(string queueName, Vhost vhost, GetLengthsCriteria lengthsCriteria = null,
            GetRatesCriteria ratesCriteria = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(queueName, nameof(queueName));
            Ensure.ArgumentNotNull(vhost, nameof(vhost));

            return GetAsync<Queue>($"queues/{SanitiseVhostName(vhost.Name)}/{SanitiseName(queueName)}",
                cancellationToken, lengthsCriteria, ratesCriteria);
        }

        public async Task<Exchange> CreateExchangeAsync(ExchangeInfo exchangeInfo, Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(vhost, nameof(exchangeInfo));
            Ensure.ArgumentNotNull(vhost, nameof(vhost));

            await PutAsync($"exchanges/{SanitiseVhostName(vhost.Name)}/{SanitiseName(exchangeInfo.GetName())}",
                exchangeInfo, cancellationToken).ConfigureAwait(false);

            return await GetExchangeAsync(SanitiseName(exchangeInfo.GetName()), vhost,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteExchangeAsync(Exchange exchange,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(exchange, nameof(exchange));

            await DeleteAsync($"exchanges/{SanitiseVhostName(exchange.Vhost)}/{SanitiseName(exchange.Name)}",
                cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<Binding>> GetBindingsWithSourceAsync(Exchange exchange,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(exchange, nameof(exchange));

            return GetAsync<IEnumerable<Binding>>(
                $"exchanges/{SanitiseVhostName(exchange.Vhost)}/{exchange.Name}/bindings/source", cancellationToken);
        }

        public Task<IEnumerable<Binding>> GetBindingsWithDestinationAsync(Exchange exchange,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(exchange, nameof(exchange));

            return GetAsync<IEnumerable<Binding>>(
                $"exchanges/{SanitiseVhostName(exchange.Vhost)}/{exchange.Name}/bindings/destination",
                cancellationToken);
        }

        public Task<PublishResult> PublishAsync(Exchange exchange, PublishInfo publishInfo,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(exchange, nameof(exchange));
            Ensure.ArgumentNotNull(publishInfo, nameof(publishInfo));

            return PostAsync<PublishInfo, PublishResult>(
                $"exchanges/{SanitiseVhostName(exchange.Vhost)}/{exchange.Name}/publish", publishInfo,
                cancellationToken);
        }

        public Task<IEnumerable<Queue>> GetQueuesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<IEnumerable<Queue>>("queues", cancellationToken);
        }

        public async Task<Queue> CreateQueueAsync(QueueInfo queueInfo, Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(queueInfo, nameof(queueInfo));
            Ensure.ArgumentNotNull(vhost, nameof(vhost));

            await PutAsync($"queues/{SanitiseVhostName(vhost.Name)}/{SanitiseName(queueInfo.GetName())}", queueInfo,
                cancellationToken).ConfigureAwait(false);

            return await GetQueueAsync(queueInfo.GetName(), vhost, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task DeleteQueueAsync(Queue queue,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(queue, nameof(queue));

            await DeleteAsync($"queues/{SanitiseVhostName(queue.Vhost)}/{SanitiseName(queue.Name)}",
                cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<Binding>> GetBindingsForQueueAsync(Queue queue,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(queue, nameof(queue));

            return GetAsync<IEnumerable<Binding>>(
                $"queues/{SanitiseVhostName(queue.Vhost)}/{SanitiseName(queue.Name)}/bindings", cancellationToken);
        }

        public async Task PurgeAsync(Queue queue, CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(queue, nameof(queue));

            await DeleteAsync($"queues/{SanitiseVhostName(queue.Vhost)}/{SanitiseName(queue.Name)}/contents",
                cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<Message>> GetMessagesFromQueueAsync(Queue queue, GetMessagesCriteria criteria,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(queue, nameof(queue));

            return PostAsync<GetMessagesCriteria, IEnumerable<Message>>(
                $"queues/{SanitiseVhostName(queue.Vhost)}/{SanitiseName(queue.Name)}/get", criteria, cancellationToken);
        }

        public Task<IEnumerable<Binding>> GetBindingsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<IEnumerable<Binding>>("bindings", cancellationToken);
        }

        public async Task CreateBinding(Exchange exchange, Queue queue, BindingInfo bindingInfo,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(exchange, nameof(exchange));
            Ensure.ArgumentNotNull(queue, nameof(queue));
            Ensure.ArgumentNotNull(bindingInfo, nameof(bindingInfo));

            await PostAsync<BindingInfo, object>(
                $"bindings/{SanitiseVhostName(queue.Vhost)}/e/{exchange.Name}/q/{SanitiseName(queue.Name)}",
                bindingInfo, cancellationToken).ConfigureAwait(false);
        }

        public async Task CreateBinding(Exchange sourceExchange, Exchange destinationExchange, BindingInfo bindingInfo,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(sourceExchange, nameof(sourceExchange));
            Ensure.ArgumentNotNull(destinationExchange, nameof(destinationExchange));
            Ensure.ArgumentNotNull(bindingInfo, nameof(bindingInfo));

            await PostAsync<BindingInfo, object>(
                $"bindings/{SanitiseVhostName(sourceExchange.Vhost)}/e/{sourceExchange.Name}/e/{destinationExchange.Name}",
                bindingInfo, cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<Binding>> GetBindingsAsync(Exchange exchange, Queue queue,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(exchange, nameof(exchange));
            Ensure.ArgumentNotNull(queue, nameof(queue));

            return GetAsync<IEnumerable<Binding>>(
                $"bindings/{SanitiseVhostName(queue.Vhost)}/e/{exchange.Name}/q/{SanitiseName(queue.Name)}",
                cancellationToken);
        }

        public Task<IEnumerable<Binding>> GetBindingsAsync(Exchange fromExchange, Exchange toExchange,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(fromExchange, nameof(fromExchange));
            Ensure.ArgumentNotNull(toExchange, nameof(toExchange));

            return GetAsync<IEnumerable<Binding>>(
                $"bindings/{SanitiseVhostName(toExchange.Vhost)}/e/{fromExchange.Name}/e/{SanitiseName(toExchange.Name)}",
                cancellationToken);
        }

        public async Task DeleteBindingAsync(Binding binding,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(binding, nameof(binding));

            await DeleteAsync(
                $"bindings/{SanitiseVhostName(binding.Vhost)}/e/{binding.Source}/q/{binding.Destination}/{RecodeBindingPropertiesKey(binding.PropertiesKey)}",
                cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<Vhost>> GetVHostsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<IEnumerable<Vhost>>("vhosts", cancellationToken);
        }

        public Task<Vhost> GetVhostAsync(string vhostName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<Vhost>($"vhosts/{SanitiseVhostName(vhostName)}", cancellationToken);
        }

        public async Task<Vhost> CreateVirtualHostAsync(string virtualHostName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(virtualHostName, nameof(virtualHostName));

            await PutAsync<string>($"vhosts/{virtualHostName}", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            return await GetVhostAsync(virtualHostName, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteVirtualHostAsync(Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(vhost, nameof(vhost));

            await DeleteAsync($"vhosts/{vhost.Name}", cancellationToken).ConfigureAwait(false);
        }

        public async Task EnableTracingAsync(Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(vhost, nameof(vhost));
            vhost.Tracing = true;
            await PutAsync<Vhost>($"vhosts/{SanitiseVhostName(vhost.Name)}", vhost, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task DisableTracingAsync(Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(vhost, nameof(vhost));
            vhost.Tracing = false;
            await PutAsync<Vhost>($"vhosts/{SanitiseVhostName(vhost.Name)}", vhost, cancellationToken)
                .ConfigureAwait(false);
        }

        public Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<IEnumerable<User>>("users", cancellationToken);
        }

        public Task<User> GetUserAsync(string userName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(userName, nameof(userName));
            return GetAsync<User>($"users/{userName}", cancellationToken);
        }

        public Task<IEnumerable<Policy>> GetPoliciesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<IEnumerable<Policy>>("policies", cancellationToken);
        }

        public async Task CreatePolicy(Policy policy, CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(policy, nameof(policy));
            if (string.IsNullOrEmpty(policy.Name))
            {
                throw new ArgumentException("Policy name is empty");
            }
            if (string.IsNullOrEmpty(policy.Vhost))
            {
                throw new ArgumentException("vhost name is empty");
            }
            if (policy.Definition == null)
            {
                throw new ArgumentException("Definition should not be null");
            }

            await PutAsync(GetPolicyUrl(policy.Name, policy.Vhost), policy, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeletePolicyAsync(string policyName, Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(policyName, nameof(policyName));
            Ensure.ArgumentNotNull(vhost, nameof(vhost));

            await DeleteAsync(GetPolicyUrl(policyName, vhost.Name), cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<Parameter>> GetParametersAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<IEnumerable<Parameter>>("parameters", cancellationToken);
        }

        public async Task CreateParameterAsync(Parameter parameter,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(parameter, nameof(parameter));
            await PutAsync(GetParameterUrl(parameter.Component, parameter.Vhost, parameter.Name), parameter.Value,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteParameterAsync(string componentName, string vhost, string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(componentName, nameof(componentName));
            Ensure.ArgumentNotNull(vhost, nameof(vhost));
            Ensure.ArgumentNotNull(name, nameof(name));

            await DeleteAsync(GetParameterUrl(componentName, vhost, name), cancellationToken).ConfigureAwait(false);
        }

        public async Task<User> CreateUserAsync(UserInfo userInfo,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(userInfo, nameof(userInfo));

            await PutAsync($"users/{userInfo.GetName()}", userInfo, cancellationToken).ConfigureAwait(false);

            return await GetUserAsync(userInfo.GetName(), cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteUserAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(user, nameof(user));

            await DeleteAsync($"users/{user.Name}", cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<Permission>> GetPermissionsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync<IEnumerable<Permission>>("permissions", cancellationToken);
        }

        public async Task CreatePermissionAsync(PermissionInfo permissionInfo,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(permissionInfo, nameof(permissionInfo));

            await PutAsync(
                $"permissions/{SanitiseVhostName(permissionInfo.GetVirtualHostName())}/{permissionInfo.GetUserName()}",
                permissionInfo, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeletePermissionAsync(Permission permission,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(permission, nameof(permission));

            await DeleteAsync($"permissions/{permission.Vhost}/{permission.User}", cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<User> ChangeUserPasswordAsync(string userName, string newPassword,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(userName, nameof(userName));
            var user = await GetUserAsync(userName, cancellationToken).ConfigureAwait(false);

            var tags = user.Tags.Split(',');
            var userInfo = new UserInfo(userName, newPassword);
            foreach (var tag in tags)
            {
                userInfo.AddTag(tag.Trim());
            }
            return await CreateUserAsync(userInfo, cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<Federation>> GetFederationAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetAsync<List<Federation>>("federation-links", cancellationToken);
        }

        public async Task<bool> IsAliveAsync(Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.ArgumentNotNull(vhost, nameof(vhost));

            var result =
                await GetAsync<AlivenessTestResult>($"aliveness-test/{SanitiseVhostName(vhost.Name)}",
                    cancellationToken).ConfigureAwait(false);
            return result.Status == "ok";
        }

        private Task<T> GetAsync<T>(
            string path,
            CancellationToken cancellationToken = default(CancellationToken),
            params object[] queryObjects)
        {
            var request = CreateRequestForPath(path, HttpMethod.Get, queryObjects);

            return httpClient.SendAsync(request, cancellationToken)
                .ContinueWithOrThrow(_ => AnalyseResponse<T>(code => code == HttpStatusCode.OK, _.Result), cancellationToken)
                    .ContinueWith(__ =>
                    {
                        request?.Dispose();
                        return __;
                    }, cancellationToken).Unwrap();

        }

        private Task<TResult> PostAsync<TItem, TResult>(
            string path,
            TItem item,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = CreateRequestForPath(path, HttpMethod.Post);

            InsertRequestBody(request, item);

            return httpClient.SendAsync(request, cancellationToken)
                .ContinueWithOrThrow(_ =>
                {
                    bool Success(HttpStatusCode statusCode) =>
                        statusCode == HttpStatusCode.OK ||
                        statusCode == HttpStatusCode.Created;

                    return AnalyseResponse<TResult>(Success, _.Result)
                        .ContinueWith(__ =>
                        {
                            request?.Dispose();
                            return __;
                        }, cancellationToken);

                },cancellationToken).Unwrap();
        }

        private Task DeleteAsync(
            string path,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = CreateRequestForPath(path, HttpMethod.Delete);
            
            return httpClient.SendAsync(request, cancellationToken)
                .ContinueWithOrThrow(_ =>
                    AnalyseResponse(statusCode => statusCode == HttpStatusCode.NoContent, _.Result)
                        .ContinueWith(__ =>
                        {
                            request?.Dispose();
                            return __;
                        }, cancellationToken).Unwrap()
                    , cancellationToken);
                
        }

        private Task PutAsync<T>(
            string path,
            T item = default(T),
            CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            var request = CreateRequestForPath(path, HttpMethod.Put);
            
            if (item != default(T))
                InsertRequestBody(request, item);

            return httpClient.SendAsync(request, cancellationToken)
                .ContinueWithOrThrow(_ =>
                {
                    bool ResponseSucceeded(HttpStatusCode statusCode) => statusCode == HttpStatusCode.OK ||
                                                                         statusCode == HttpStatusCode.Created ||
                                                                         statusCode == HttpStatusCode.NoContent;

                    return AnalyseResponse(ResponseSucceeded, _.Result)
                            .ContinueWith(__ =>
                            {
                                request?.Dispose();
                                return __;
                            }, cancellationToken).Unwrap();
                }, cancellationToken);
            
        }

        private Task AnalyseResponse(
            Func<HttpStatusCode, bool> success,
            HttpResponseMessage responseMessage)
        {
            var httpStatusCode = responseMessage.StatusCode;
            try
            {
                if (success(httpStatusCode))
                    return CompletedTask;
                throw new UnexpectedHttpStatusCodeException(httpStatusCode);
            }
            finally
            {
                responseMessage.Dispose();
            }
        }

        private static Task<TResult> AnalyseResponse<TResult>(
            Func<HttpStatusCode, bool> success,
            HttpResponseMessage responseMessage)
        {
            var httpStatusCode = responseMessage.StatusCode;
            try
            {
                if (success(httpStatusCode))
                    return DeserializeResponseAsync<TResult>(responseMessage);
                throw new UnexpectedHttpStatusCodeException(httpStatusCode);
            }
            finally
            {
                responseMessage.Dispose();
            }
        }

        private static void InsertRequestBody<T>(HttpRequestMessage request, T item)
        {
            if (!request.Headers.Accept.Contains(JsonMediaTypeHeaderValue))
                request.Headers.Accept.Add(JsonMediaTypeHeaderValue);

            var body = JsonConvert.SerializeObject(item, Settings);
            var content = new StringContent(body);

            content.Headers.ContentType = JsonMediaTypeHeaderValue;
            request.Content = content;
        }

        private static string GetPolicyUrl(string policyName, string vhost)
        {
            return $"policies/{SanitiseVhostName(vhost)}/{policyName}";
        }

        private static string GetParameterUrl(string componentName, string vhost, string parameterName)
        {
            return $"parameters/{componentName}/{SanitiseVhostName(vhost)}/{parameterName}";
        }

        private static Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync()
                .ContinueWith(_ => JsonConvert.DeserializeObject<T>(_.Result, Settings));
        }

        private HttpRequestMessage CreateRequestForPath(string path, HttpMethod httpMethod,
            IReadOnlyCollection<object> queryObjects = null)
        {
            var queryString = BuildQueryString(queryObjects);

            var uri = new Uri($"{HostUrl}:{PortNumber}/api/{path}{queryString}");
            var request = new HttpRequestMessage(httpMethod, uri);

            configureRequest(request);

            return request;
        }

        // Very simple query-string builder. 
        private static string BuildQueryString(IReadOnlyCollection<object> queryObjects)
        {
            if (queryObjects == null || queryObjects.Count == 0)
                return string.Empty;

            var queryStringBuilder = new StringBuilder("?");
            var first = true;
            // One or more query objects can be used to build the query
            foreach (var query in queryObjects)
            {
                if (query == null)
                    continue;
                // All public properties are added to the query on the format property_name=value
                var type = query.GetType();
                foreach (var prop in type.GetProperties())
                {
                    var name = Regex.Replace(prop.Name, "([a-z])([A-Z])", "$1_$2").ToLower();
                    var value = prop.GetValue(query, null);
                    if (!first)
                        queryStringBuilder.Append("&");
                    queryStringBuilder.AppendFormat("{0}={1}", name, value ?? string.Empty);
                    first = false;
                }
            }
            return queryStringBuilder.ToString();
        }

        private static string SanitiseVhostName(string vhostName)
        {
            return vhostName.Replace("/", "%2f");
        }

        private static string SanitiseName(string queueName)
        {
            return queueName.Replace("+", "%2B").Replace("#", "%23").Replace("/", "%2f");
        }

        private static string RecodeBindingPropertiesKey(string propertiesKey)
        {
            return propertiesKey.Replace("%5F", "%255F");
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}