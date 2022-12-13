using System.Net;
using System.Net.Http.Headers;
using EasyNetQ.Management.Client.Internals;
using EasyNetQ.Management.Client.Model;
using EasyNetQ.Management.Client.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace EasyNetQ.Management.Client;

public class ManagementClient : IManagementClient
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(20);

    private static readonly RelativePath Api = new("api");
    private static readonly RelativePath Vhosts = Api / "vhosts";
    private static readonly RelativePath AlivenessTest = Api / "aliveness-test";
    private static readonly RelativePath Connections = Api / "connections";
    private static readonly RelativePath Consumers = Api / "consumers";
    private static readonly RelativePath Channels = Api / "channels";
    private static readonly RelativePath Users = Api / "users";
    private static readonly RelativePath Permissions = Api / "permissions";
    private static readonly RelativePath Parameters = Api / "parameters";
    private static readonly RelativePath Bindings = Api / "bindings";
    private static readonly RelativePath Queues = Api / "queues";
    private static readonly RelativePath Exchanges = Api / "exchanges";
    private static readonly RelativePath TopicPermissions = Api / "topic-permissions";
    private static readonly RelativePath Policies = Api / "policies";
    private static readonly RelativePath FederationLinks = Api / "federation-links";
    private static readonly RelativePath Overview = Api / "overview";
    private static readonly RelativePath Nodes = Api / "nodes";
    private static readonly RelativePath Definitions = Api / "definitions";

    private static readonly MediaTypeWithQualityHeaderValue JsonMediaTypeHeaderValue = new("application/json");

    public static readonly JsonSerializerSettings Settings;

    private readonly HttpClient httpClient;
    private readonly Action<HttpRequestMessage>? configureHttpRequestMessage;

    static ManagementClient()
    {
        Settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy(true, true)
            },
            NullValueHandling = NullValueHandling.Ignore
        };

        Settings.Converters.Add(new PropertyConverter());
        Settings.Converters.Add(new MessageStatsOrEmptyArrayConverter());
        Settings.Converters.Add(new QueueTotalsOrEmptyArrayConverter());
        Settings.Converters.Add(new StringEnumConverter { NamingStrategy = new SnakeCaseNamingStrategy(true, true) });
        Settings.Converters.Add(new HaParamsConverter());
    }

    [Obsolete("Please use another constructor")]
    public ManagementClient(
        string hostUrl,
        string username,
        string password,
        int portNumber = 15672,
        TimeSpan? timeout = null,
        Action<HttpRequestMessage>? configureHttpRequestMessage = null,
        bool ssl = false,
        Action<HttpClientHandler>? configureHttpClientHandler = null
    ) : this(
        LegacyEndpointBuilder.Build(hostUrl, portNumber, ssl),
        username,
        password,
        timeout,
        configureHttpRequestMessage,
        configureHttpClientHandler
    )
    {
    }

    public ManagementClient(
        Uri endpoint,
        string username,
        string password,
        TimeSpan? timeout = null,
        Action<HttpRequestMessage>? configureHttpRequestMessage = null,
        Action<HttpClientHandler>? configureHttpClientHandler = null
    )
    {
        if (!endpoint.IsAbsoluteUri) throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, "Endpoint should be absolute");

        this.configureHttpRequestMessage = configureHttpRequestMessage;

        var httpHandler = new HttpClientHandler { Credentials = new NetworkCredential(username, password) };
        configureHttpClientHandler?.Invoke(httpHandler);
        httpClient = new HttpClient(httpHandler) { Timeout = timeout ?? DefaultTimeout, BaseAddress = endpoint };
    }

    public Uri Endpoint => httpClient.BaseAddress!;

    public Task<Overview> GetOverviewAsync(
        GetLengthsCriteria? lengthsCriteria = null,
        GetRatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryParameters = MergeQueryParameters(
            lengthsCriteria?.ToQueryParameters(), ratesCriteria?.ToQueryParameters()
        );
        return GetAsync<Overview>(Overview, queryParameters, cancellationToken);
    }

    public Task<IReadOnlyList<Node>> GetNodesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Node>>(Nodes, cancellationToken);
    }

    public Task<Definitions> GetDefinitionsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<Definitions>(Definitions, cancellationToken);
    }

    public Task<IReadOnlyList<Connection>> GetConnectionsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Connection>>(Connections, cancellationToken);
    }

    public Task CloseConnectionAsync(Connection connection, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(Connections / connection.Name, cancellationToken);
    }

    public Task<IReadOnlyList<Channel>> GetChannelsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Channel>>(Channels, cancellationToken);
    }

    public Task<IReadOnlyList<Channel>> GetChannelsAsync(Connection connection, CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Channel>>(Connections / connection.Name / "channels", cancellationToken);
    }

    public Task<Channel> GetChannelAsync(
        string channelName,
        GetRatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<Channel>(Channels / channelName, ratesCriteria?.ToQueryParameters(), cancellationToken);
    }

    public Task<IReadOnlyList<Exchange>> GetExchangesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Exchange>>(Exchanges, cancellationToken);
    }

    public Task<Exchange> GetExchangeAsync(
        string exchangeName,
        Vhost vhost,
        GetRatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<Exchange>(
            Exchanges / vhost.Name / exchangeName,
            ratesCriteria?.ToQueryParameters(),
            cancellationToken
        );
    }

    public Task<Queue> GetQueueAsync(
        string queueName,
        Vhost vhost,
        GetLengthsCriteria? lengthsCriteria = null,
        GetRatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryParameters = MergeQueryParameters(
            lengthsCriteria?.ToQueryParameters(), ratesCriteria?.ToQueryParameters()
        );
        return GetAsync<Queue>(
            Queues / vhost.Name / queueName,
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
        await PutAsync(
            Exchanges / vhost.Name / exchangeInfo.GetName(),
            exchangeInfo,
            cancellationToken
        ).ConfigureAwait(false);
    }

    public Task DeleteExchangeAsync(Exchange exchange, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            Exchanges / exchange.Vhost / exchange.Name,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsWithSourceAsync(
        Exchange exchange,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<IReadOnlyList<Binding>>(
            Exchanges / exchange.Vhost / exchange.Name / "bindings" / "source",
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsWithDestinationAsync(
        Exchange exchange,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<IReadOnlyList<Binding>>(
            Exchanges / exchange.Vhost / exchange.Name / "bindings" / "destination",
            cancellationToken
        );
    }

    public Task<PublishResult> PublishAsync(
        Exchange exchange,
        PublishInfo publishInfo,
        CancellationToken cancellationToken = default
    )
    {
        return PostAsync<PublishInfo, PublishResult>(
            Exchanges / exchange.Vhost / exchange.Name / "publish",
            publishInfo,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Queue>> GetQueuesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Queue>>(Queues, cancellationToken);
    }

    public Task<IReadOnlyList<Queue>> GetQueuesAsync(Vhost vhost, CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Queue>>(Queues / vhost.Name, cancellationToken);
    }

    public async Task CreateQueueAsync(
        QueueInfo queueInfo,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        await PutAsync(
            Queues / vhost.Name / queueInfo.GetName(),
            queueInfo,
            cancellationToken
        ).ConfigureAwait(false);
    }

    public Task DeleteQueueAsync(Queue queue, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            Queues / queue.Vhost / queue.Name, cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsForQueueAsync(Queue queue, CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Binding>>(
            Queues / queue.Vhost / queue.Name / "bindings", cancellationToken
        );
    }

    public Task PurgeAsync(Queue queue, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            Queues / queue.Vhost / queue.Name / "contents", cancellationToken
        );
    }

    public Task<IReadOnlyList<Message>> GetMessagesFromQueueAsync(
        Queue queue,
        GetMessagesCriteria criteria,
        CancellationToken cancellationToken = default
    )
    {
        return PostAsync<GetMessagesCriteria, IReadOnlyList<Message>>(
            Queues / queue.Vhost / queue.Name / "get",
            criteria,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Binding>>(Bindings, cancellationToken);
    }

    public Task CreateBindingAsync(
        Exchange exchange,
        Queue queue,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    )
    {
        return PostAsync<BindingInfo, object>(
            Bindings / queue.Vhost / "e" / exchange.Name / "q" / queue.Name,
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
        return PostAsync<BindingInfo, object>(
            Bindings / sourceExchange.Vhost / "e" / sourceExchange.Name / "e" / destinationExchange.Name,
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
        return GetAsync<IReadOnlyList<Binding>>(
            Bindings / queue.Vhost / "e" / exchange.Name / "q" / queue.Name,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsAsync(
        Exchange fromExchange,
        Exchange toExchange,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<IReadOnlyList<Binding>>(
            Bindings / toExchange.Vhost / "e" / fromExchange.Name / "e" / toExchange.Name,
            cancellationToken
        );
    }

    public Task DeleteBindingAsync(Binding binding, CancellationToken cancellationToken = default)
    {
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
            Bindings / binding.Vhost / "e" / binding.Source / binding.DestinationType[0] / binding.Destination / binding.PropertiesKey,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Vhost>> GetVhostsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Vhost>>(Vhosts, cancellationToken);
    }

    public Task<Vhost> GetVhostAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return GetAsync<Vhost>(Vhosts / vhostName, cancellationToken);
    }

    public Task CreateVhostAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return PutAsync<string>(Vhosts / vhostName, cancellationToken: cancellationToken);
    }

    public Task DeleteVhostAsync(Vhost vhost, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(Vhosts / vhost.Name, cancellationToken);
    }

    public Task EnableTracingAsync(Vhost vhost, CancellationToken cancellationToken = default)
    {
        vhost.Tracing = true;
        return PutAsync(Vhosts / vhost.Name, vhost, cancellationToken);
    }

    public Task DisableTracingAsync(Vhost vhost, CancellationToken cancellationToken = default)
    {
        vhost.Tracing = false;
        return PutAsync(Vhosts / vhost.Name, vhost, cancellationToken);
    }

    public Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<User>>(Users, cancellationToken);
    }

    public Task<User> GetUserAsync(string userName, CancellationToken cancellationToken = default)
    {
        return GetAsync<User>(Users / userName, cancellationToken);
    }

    public Task<IReadOnlyList<Policy>> GetPoliciesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Policy>>(Policies, cancellationToken);
    }

    public Task CreatePolicyAsync(Policy policy, CancellationToken cancellationToken = default)
    {
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

        return PutAsync(Policies / policy.Vhost / policy.Name, policy, cancellationToken);
    }

    public Task DeletePolicyAsync(string policyName, Vhost vhost, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(Policies / vhost.Name / policyName, cancellationToken);
    }

    public Task<IReadOnlyList<Parameter>> GetParametersAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Parameter>>(Parameters, cancellationToken);
    }

    public Task CreateParameterAsync(Parameter parameter, CancellationToken cancellationToken = default)
    {
        return PutAsync(
            Parameters / parameter.Component / parameter.Vhost / parameter.Name,
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
        return DeleteAsync(Parameters / componentName / vhost / name, cancellationToken);
    }

    public async Task CreateUserAsync(UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        await PutAsync(Users / userInfo.Name, userInfo, cancellationToken).ConfigureAwait(false);
    }

    public Task DeleteUserAsync(User user, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(Users / user.Name, cancellationToken);
    }

    public Task<IReadOnlyList<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Permission>>(Permissions, cancellationToken);
    }

    public Task CreatePermissionAsync(PermissionInfo permissionInfo, CancellationToken cancellationToken = default)
    {
        return PutAsync(
            Permissions / permissionInfo.GetVirtualHostName() / permissionInfo.GetUserName(),
            permissionInfo,
            cancellationToken
        );
    }

    public Task DeletePermissionAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(Permissions / permission.Vhost / permission.User, cancellationToken);
    }

    public Task<IReadOnlyList<TopicPermission>> GetTopicPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<TopicPermission>>(TopicPermissions, cancellationToken);
    }

    public Task CreateTopicPermissionAsync(TopicPermissionInfo topicPermissionInfo, CancellationToken cancellationToken = default)
    {
        return PutAsync(
            TopicPermissions / topicPermissionInfo.GetVirtualHostName() / topicPermissionInfo.GetUserName(),
            topicPermissionInfo,
            cancellationToken
        );
    }

    public Task DeleteTopicPermissionAsync(TopicPermission topicPermission, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(TopicPermissions / topicPermission.Vhost / topicPermission.User, cancellationToken);
    }

    public Task<IReadOnlyList<Federation>> GetFederationsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Federation>>(FederationLinks, cancellationToken);
    }

    public async Task<bool> IsAliveAsync(Vhost vhost, CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<AlivenessTestResult>(AlivenessTest / vhost.Name, cancellationToken).ConfigureAwait(false);
        return result.Status == "ok";
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }

    public Task<IReadOnlyList<Consumer>> GetConsumersAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Consumer>>(Consumers, cancellationToken);
    }

    private Task<T> GetAsync<T>(RelativePath path, CancellationToken cancellationToken = default) => GetAsync<T>(path, null, cancellationToken);

    private async Task<T> GetAsync<T>(
        RelativePath path,
        IReadOnlyDictionary<string, string>? queryParameters,
        CancellationToken cancellationToken = default
    )
    {
        using var request = CreateRequestForPath(HttpMethod.Get, path, queryParameters);
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        return await DeserializeResponseAsync<T>(c => c == HttpStatusCode.OK, response).ConfigureAwait(false);
    }

    private async Task<TResult> PostAsync<TItem, TResult>(
        RelativePath path,
        TItem item,
        CancellationToken cancellationToken = default
    )
    {
        using var request = CreateRequestForPath(HttpMethod.Post, path);

        InsertRequestBody(request, item);

        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        return await DeserializeResponseAsync<TResult>(
            c => c is HttpStatusCode.OK or HttpStatusCode.Created, response
        ).ConfigureAwait(false);
    }

    private async Task DeleteAsync(RelativePath path, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequestForPath(HttpMethod.Delete, path);
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        await DeserializeResponseAsync(c => c == HttpStatusCode.NoContent, response).ConfigureAwait(false);
    }

    private async Task PutAsync<T>(
        RelativePath path,
        T? item = default,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        using var request = CreateRequestForPath(HttpMethod.Put, path);

        if (item != default) InsertRequestBody(request, item);

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
        return JsonConvert.DeserializeObject<T>(content, Settings)!;
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

    private HttpRequestMessage CreateRequestForPath(
        HttpMethod httpMethod,
        in RelativePath path,
        IReadOnlyDictionary<string, string>? queryParameters = null
    )
    {
        var httpRequestMessage = new HttpRequestMessage(httpMethod, QueryStringHelpers.AddQueryString(path.Build(), queryParameters));
        configureHttpRequestMessage?.Invoke(httpRequestMessage);
        return httpRequestMessage;
    }

    private static IReadOnlyDictionary<string, string>? MergeQueryParameters(params IReadOnlyDictionary<string, string>?[]? multipleQueryParameters)
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
}
