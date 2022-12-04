using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using EasyNetQ.Management.Client.Model;
using EasyNetQ.Management.Client.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace EasyNetQ.Management.Client;

public class ManagementClient : IManagementClient
{
    private static readonly Regex ParameterNameRegex = new("([a-z])([A-Z])", RegexOptions.Compiled);

    private static readonly MediaTypeWithQualityHeaderValue JsonMediaTypeHeaderValue = new("application/json");

    public static readonly JsonSerializerSettings Settings;

    private readonly Action<HttpRequestMessage> configureRequest;
    private readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(20);
    private readonly HttpClient httpClient;

    private readonly Regex urlRegex = new(@"^(http|https):\/\/\[?.+\w\]?$", RegexOptions.Compiled | RegexOptions.Singleline);

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
        Settings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
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
        bool ssl = false,
        Action<HttpClientHandler> handlerConfigurator = null
    )
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

        configureRequest ??= _ => { };

        HostUrl = hostUrl;
        Username = username;
        PortNumber = portNumber;
        this.configureRequest = configureRequest;

        var httpHandler = new HttpClientHandler
        {
            Credentials = new NetworkCredential(username, password)
        };

        handlerConfigurator?.Invoke(httpHandler);

        httpClient = new HttpClient(httpHandler) { Timeout = timeout ?? defaultTimeout };
    }

    public Task<Overview> GetOverviewAsync(
        GetLengthsCriteria lengthsCriteria = null,
        GetRatesCriteria ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryParameters = MergeQueryParameters(
            lengthsCriteria?.ToQueryParameters(), ratesCriteria?.ToQueryParameters()
        );
        return GetAsync<Overview>("overview", queryParameters, cancellationToken);
    }

    public Task<IReadOnlyList<Node>> GetNodesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Node>>("nodes", cancellationToken);
    }

    public Task<Definitions> GetDefinitionsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<Definitions>("definitions", cancellationToken);
    }

    public Task<IReadOnlyList<Connection>> GetConnectionsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Connection>>("connections", cancellationToken);
    }

    public Task CloseConnectionAsync(Connection connection, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(connection, nameof(connection));
        return DeleteAsync($"connections/{connection.Name}", cancellationToken);
    }

    public Task<IReadOnlyList<Channel>> GetChannelsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Channel>>("channels", cancellationToken);
    }

    public Task<IReadOnlyList<Channel>> GetChannelsAsync(Connection connection, CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Channel>>($"connections/{connection.Name}/channels", cancellationToken);
    }

    public Task<Channel> GetChannelAsync(
        string channelName,
        GetRatesCriteria ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(channelName, nameof(channelName));

        return GetAsync<Channel>($"channels/{channelName}", ratesCriteria?.ToQueryParameters(), cancellationToken);
    }

    public Task<IReadOnlyList<Exchange>> GetExchangesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Exchange>>("exchanges", cancellationToken);
    }

    public Task<Exchange> GetExchangeAsync(
        string exchangeName,
        Vhost vhost,
        GetRatesCriteria ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(exchangeName, nameof(exchangeName));
        Ensure.ArgumentNotNull(vhost, nameof(vhost));

        return GetAsync<Exchange>($"exchanges/{SanitiseVhostName(vhost.Name)}/{exchangeName}", ratesCriteria?.ToQueryParameters(), cancellationToken);
    }

    public Task<Queue> GetQueueAsync(
        string queueName,
        Vhost vhost,
        GetLengthsCriteria lengthsCriteria = null,
        GetRatesCriteria ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(queueName, nameof(queueName));
        Ensure.ArgumentNotNull(vhost, nameof(vhost));

        var queryParameters = MergeQueryParameters(
            lengthsCriteria?.ToQueryParameters(), ratesCriteria?.ToQueryParameters()
        );
        return GetAsync<Queue>(
            $"queues/{SanitiseVhostName(vhost.Name)}/{SanitiseName(queueName)}",
            queryParameters,
            cancellationToken
        );
    }

    public async Task CreateExchangeAsync(
        ExchangeInfo exchangeInfo,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(vhost, nameof(exchangeInfo));
        Ensure.ArgumentNotNull(vhost, nameof(vhost));

        await PutAsync
        (
            $"exchanges/{SanitiseVhostName(vhost.Name)}/{SanitiseName(exchangeInfo.GetName())}",
            exchangeInfo,
            cancellationToken
        ).ConfigureAwait(false);
    }

    public Task DeleteExchangeAsync(Exchange exchange, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(exchange, nameof(exchange));

        return DeleteAsync(
            $"exchanges/{SanitiseVhostName(exchange.Vhost)}/{SanitiseName(exchange.Name)}",
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsWithSourceAsync(
        Exchange exchange,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(exchange, nameof(exchange));

        return GetAsync<IReadOnlyList<Binding>>(
            $"exchanges/{SanitiseVhostName(exchange.Vhost)}/{exchange.Name}/bindings/source",
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsWithDestinationAsync(
        Exchange exchange,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(exchange, nameof(exchange));

        return GetAsync<IReadOnlyList<Binding>>(
            $"exchanges/{SanitiseVhostName(exchange.Vhost)}/{exchange.Name}/bindings/destination",
            cancellationToken
        );
    }

    public Task<PublishResult> PublishAsync(
        Exchange exchange,
        PublishInfo publishInfo,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(exchange, nameof(exchange));
        Ensure.ArgumentNotNull(publishInfo, nameof(publishInfo));

        return PostAsync<PublishInfo, PublishResult>(
            $"exchanges/{SanitiseVhostName(exchange.Vhost)}/{exchange.Name}/publish",
            publishInfo,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Queue>> GetQueuesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Queue>>("queues", cancellationToken);
    }

    public Task<IReadOnlyList<Queue>> GetQueuesAsync(Vhost vhost, CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Queue>>($"queues/{SanitiseVhostName(vhost.Name)}", cancellationToken);
    }

    public async Task CreateQueueAsync(
        QueueInfo queueInfo,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(queueInfo, nameof(queueInfo));
        Ensure.ArgumentNotNull(vhost, nameof(vhost));

        await PutAsync(
            $"queues/{SanitiseVhostName(vhost.Name)}/{SanitiseName(queueInfo.GetName())}",
            queueInfo,
            cancellationToken
        ).ConfigureAwait(false);
    }

    public Task DeleteQueueAsync(Queue queue, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(queue, nameof(queue));

        return DeleteAsync(
            $"queues/{SanitiseVhostName(queue.Vhost)}/{SanitiseName(queue.Name)}", cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsForQueueAsync(Queue queue, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(queue, nameof(queue));

        return GetAsync<IReadOnlyList<Binding>>(
            $"queues/{SanitiseVhostName(queue.Vhost)}/{SanitiseName(queue.Name)}/bindings", cancellationToken
        );
    }

    public Task PurgeAsync(Queue queue, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(queue, nameof(queue));

        return DeleteAsync(
            $"queues/{SanitiseVhostName(queue.Vhost)}/{SanitiseName(queue.Name)}/contents", cancellationToken
        );
    }

    public Task<IReadOnlyList<Message>> GetMessagesFromQueueAsync(
        Queue queue,
        GetMessagesCriteria criteria,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(queue, nameof(queue));

        return PostAsync<GetMessagesCriteria, IReadOnlyList<Message>>(
            $"queues/{SanitiseVhostName(queue.Vhost)}/{SanitiseName(queue.Name)}/get",
            criteria,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Binding>>("bindings", cancellationToken);
    }

    public Task CreateBindingAsync(
        Exchange exchange,
        Queue queue,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(exchange, nameof(exchange));
        Ensure.ArgumentNotNull(queue, nameof(queue));
        Ensure.ArgumentNotNull(bindingInfo, nameof(bindingInfo));

        return PostAsync<BindingInfo, object>(
            $"bindings/{SanitiseVhostName(queue.Vhost)}/e/{exchange.Name}/q/{SanitiseName(queue.Name)}",
            bindingInfo,
            cancellationToken
        );
    }

    public Task CreateBindingAsync(
        Exchange sourceExchange,
        Exchange destinationExchange,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(sourceExchange, nameof(sourceExchange));
        Ensure.ArgumentNotNull(destinationExchange, nameof(destinationExchange));
        Ensure.ArgumentNotNull(bindingInfo, nameof(bindingInfo));

        return PostAsync<BindingInfo, object>(
            $"bindings/{SanitiseVhostName(sourceExchange.Vhost)}/e/{sourceExchange.Name}/e/{destinationExchange.Name}",
            bindingInfo,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsAsync(
        Exchange exchange,
        Queue queue,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(exchange, nameof(exchange));
        Ensure.ArgumentNotNull(queue, nameof(queue));

        return GetAsync<IReadOnlyList<Binding>>(
            $"bindings/{SanitiseVhostName(queue.Vhost)}/e/{exchange.Name}/q/{SanitiseName(queue.Name)}",
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsAsync(
        Exchange fromExchange,
        Exchange toExchange,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(fromExchange, nameof(fromExchange));
        Ensure.ArgumentNotNull(toExchange, nameof(toExchange));

        return GetAsync<IReadOnlyList<Binding>>(
            $"bindings/{SanitiseVhostName(toExchange.Vhost)}/e/{fromExchange.Name}/e/{SanitiseName(toExchange.Name)}",
            cancellationToken
        );
    }

    public Task DeleteBindingAsync(Binding binding, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(binding, nameof(binding));

        if (string.IsNullOrEmpty(binding.Source))
        {
            throw new ArgumentException("Empty binding source isn't supported.");
        }

        if (string.IsNullOrEmpty(binding.Destination))
        {
            throw new ArgumentException("Empty binding destination isn't supported.");
        }

        if (string.IsNullOrEmpty(binding.DestinationType))
        {
            throw new ArgumentException("Empty binding destination type isn't supported.");
        }

        return DeleteAsync(
            string.Format(
                "bindings/{0}/e/{1}/{2}/{3}/{4}",
                SanitiseVhostName(binding.Vhost),
                binding.Source,
                binding.DestinationType[0], // e for exchange or q for queue
                binding.Destination,
                RecodeBindingPropertiesKey(binding.PropertiesKey)
            ),
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Vhost>> GetVhostsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Vhost>>("vhosts", cancellationToken);
    }

    public Task<Vhost> GetVhostAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return GetAsync<Vhost>($"vhosts/{SanitiseVhostName(vhostName)}", cancellationToken);
    }

    public Task CreateVhostAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(vhostName, nameof(vhostName));

        return PutAsync<string>($"vhosts/{SanitiseVhostName(vhostName)}", cancellationToken: cancellationToken);
    }

    public Task DeleteVhostAsync(Vhost vhost, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(vhost, nameof(vhost));

        return DeleteAsync($"vhosts/{SanitiseVhostName(vhost.Name)}", cancellationToken);
    }

    public Task EnableTracingAsync(Vhost vhost, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(vhost, nameof(vhost));
        vhost.Tracing = true;
        return PutAsync($"vhosts/{SanitiseVhostName(vhost.Name)}", vhost, cancellationToken);
    }

    public Task DisableTracingAsync(Vhost vhost, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(vhost, nameof(vhost));
        vhost.Tracing = false;
        return PutAsync($"vhosts/{SanitiseVhostName(vhost.Name)}", vhost, cancellationToken);
    }

    public Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<User>>("users", cancellationToken);
    }

    public Task<User> GetUserAsync(string userName, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(userName, nameof(userName));
        return GetAsync<User>($"users/{userName}", cancellationToken);
    }

    public Task<IReadOnlyList<Policy>> GetPoliciesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Policy>>("policies", cancellationToken);
    }

    public Task CreatePolicyAsync(Policy policy, CancellationToken cancellationToken = default)
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

        return PutAsync(GetPolicyUrl(policy.Name, policy.Vhost), policy, cancellationToken);
    }

    public Task DeletePolicyAsync(string policyName, Vhost vhost, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(policyName, nameof(policyName));
        Ensure.ArgumentNotNull(vhost, nameof(vhost));

        return DeleteAsync(GetPolicyUrl(policyName, vhost.Name), cancellationToken);
    }

    public Task<IReadOnlyList<Parameter>> GetParametersAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Parameter>>("parameters", cancellationToken);
    }

    public Task CreateParameterAsync(Parameter parameter, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(parameter, nameof(parameter));

        return PutAsync(
            GetParameterUrl(parameter.Component, parameter.Vhost, parameter.Name),
            parameter.Value,
            cancellationToken
        );
    }

    public Task DeleteParameterAsync(
        string componentName,
        string vhost,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        Ensure.ArgumentNotNull(componentName, nameof(componentName));
        Ensure.ArgumentNotNull(vhost, nameof(vhost));
        Ensure.ArgumentNotNull(name, nameof(name));

        return DeleteAsync(GetParameterUrl(componentName, vhost, name), cancellationToken);
    }

    public async Task CreateUserAsync(UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(userInfo, nameof(userInfo));

        await PutAsync($"users/{userInfo.GetName()}", userInfo, cancellationToken).ConfigureAwait(false);
    }

    public Task DeleteUserAsync(User user, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(user, nameof(user));

        return DeleteAsync($"users/{user.Name}", cancellationToken);
    }

    public Task<IReadOnlyList<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Permission>>("permissions", cancellationToken);
    }

    public Task CreatePermissionAsync(PermissionInfo permissionInfo, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(permissionInfo, nameof(permissionInfo));

        return PutAsync(
            $"permissions/{SanitiseVhostName(permissionInfo.GetVirtualHostName())}/{permissionInfo.GetUserName()}",
            permissionInfo,
            cancellationToken
        );
    }

    public Task DeletePermissionAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(permission, nameof(permission));

        return DeleteAsync($"permissions/{permission.Vhost}/{permission.User}", cancellationToken);
    }

    public Task<IReadOnlyList<TopicPermission>> GetTopicPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<TopicPermission>>("topic-permissions", cancellationToken);
    }

    public Task CreateTopicPermissionAsync(TopicPermissionInfo topicPermissionInfo, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(topicPermissionInfo, nameof(topicPermissionInfo));

        return PutAsync(
            $"topic-permissions/{SanitiseVhostName(topicPermissionInfo.GetVirtualHostName())}/{topicPermissionInfo.GetUserName()}",
            topicPermissionInfo, cancellationToken
        );
    }

    public Task DeleteTopicPermissionAsync(TopicPermission topicPermission, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(topicPermission, nameof(topicPermission));

        return DeleteAsync($"topic-permissions/{topicPermission.Vhost}/{topicPermission.User}", cancellationToken);
    }

    public Task<List<Federation>> GetFederationAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<Federation>>("federation-links", cancellationToken);
    }

    public async Task<bool> IsAliveAsync(Vhost vhost, CancellationToken cancellationToken = default)
    {
        Ensure.ArgumentNotNull(vhost, nameof(vhost));

        var result = await GetAsync<AlivenessTestResult>(
            $"aliveness-test/{SanitiseVhostName(vhost.Name)}", cancellationToken
        ).ConfigureAwait(false);
        return result.Status == "ok";
    }

    private Task<T> GetAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        return GetAsync<T>(path, null, cancellationToken);
    }


    private async Task<T> GetAsync<T>(
        string path,
        IReadOnlyDictionary<string, string> queryParameters,
        CancellationToken cancellationToken = default
    )
    {
        using var request = CreateRequestForPath(HttpMethod.Get, path, BuildQueryString(queryParameters));
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        return await DeserializeResponseAsync<T>(c => c == HttpStatusCode.OK, response).ConfigureAwait(false);
    }

    private async Task<TResult> PostAsync<TItem, TResult>(
        string path,
        TItem item,
        CancellationToken cancellationToken = default
    )
    {
        using var request = CreateRequestForPath(HttpMethod.Post, path, string.Empty);

        InsertRequestBody(request, item);

        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        return await DeserializeResponseAsync<TResult>(
            c => c is HttpStatusCode.OK or HttpStatusCode.Created, response
        ).ConfigureAwait(false);
    }

    private async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequestForPath(HttpMethod.Delete, path, string.Empty);
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        await DeserializeResponseAsync(c => c == HttpStatusCode.NoContent, response).ConfigureAwait(false);
    }

    private async Task PutAsync<T>(
        string path,
        T item = default,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        using var request = CreateRequestForPath(HttpMethod.Put, path, string.Empty);

        if (item != null) InsertRequestBody(request, item);

        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        await DeserializeResponseAsync(
            s => s is HttpStatusCode.OK or HttpStatusCode.Created or HttpStatusCode.NoContent, response
        ).ConfigureAwait(false);
    }

    private static Task DeserializeResponseAsync(Func<HttpStatusCode, bool> success, HttpResponseMessage response)
    {
        return success(response.StatusCode)
            ? Task.CompletedTask
            : Task.FromException(new UnexpectedHttpStatusCodeException(response.StatusCode));
    }

    private static async Task<T> DeserializeResponseAsync<T>(Func<HttpStatusCode, bool> success, HttpResponseMessage response)
    {
        if (!success(response.StatusCode))
            throw new UnexpectedHttpStatusCodeException(response.StatusCode);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonConvert.DeserializeObject<T>(content, Settings);
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

    private HttpRequestMessage CreateRequestForPath(HttpMethod httpMethod, string path, string query)
    {
        var uri = new Uri($"{HostUrl}:{PortNumber}/api/{path}{query ?? string.Empty}");
        var request = new HttpRequestMessage(httpMethod, uri);
        configureRequest(request);
        return request;
    }

    private static string BuildQueryString(IReadOnlyDictionary<string, string> queryParameters)
    {
        if (queryParameters == null || queryParameters.Count == 0)
            return string.Empty;

        var queryStringBuilder = new StringBuilder("?");
        var first = true;
        foreach (var parameter in queryParameters)
        {
            if (!first)
                queryStringBuilder.Append("&");
            var name = ParameterNameRegex.Replace(parameter.Key, "$1_$2").ToLower();
            var value = parameter.Value ?? "";
            queryStringBuilder.AppendFormat("{0}={1}", name, value);
            first = false;
        }

        return queryStringBuilder.ToString();
    }

    private static string SanitiseVhostName(string vhostName)
    {
        return vhostName.Replace("/", "%2f");
    }

    private static string SanitiseName(string queueName)
    {
        return queueName.Replace("+", "%2B")
            .Replace("#", "%23")
            .Replace("/", "%2f")
            .Replace(":", "%3A")
            .Replace("[", "%5B")
            .Replace("]", "%5D");
    }

    private static string RecodeBindingPropertiesKey(string propertiesKey)
    {
        return propertiesKey.Replace("%5F", "%255F");
    }

    private static IReadOnlyDictionary<string, string> MergeQueryParameters(params IReadOnlyDictionary<string, string>[] multipleQueryParameters)
    {
        if (multipleQueryParameters == null || multipleQueryParameters.Length == 0)
            return null;

        var mergedQueryParameters = new Dictionary<string, string>();
        foreach (var queryParameters in multipleQueryParameters)
        {
            if (queryParameters == null)
                continue;

            foreach (var kvp in queryParameters)
                mergedQueryParameters[kvp.Key] = kvp.Value;
        }

        return mergedQueryParameters;
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }

    public Task<IReadOnlyList<Consumer>> GetConsumersAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Consumer>>("consumers", cancellationToken);
    }
}
