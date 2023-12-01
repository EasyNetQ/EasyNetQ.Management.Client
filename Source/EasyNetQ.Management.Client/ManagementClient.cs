using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Internals;
using EasyNetQ.Management.Client.Model;
using EasyNetQ.Management.Client.Serialization;

#if NET6_0
using HttpHandler = System.Net.Http.SocketsHttpHandler;
#else
using HttpHandler = System.Net.Http.HttpClientHandler;
#endif

namespace EasyNetQ.Management.Client;

public class ManagementClient : IManagementClient
{
    private static readonly MediaTypeWithQualityHeaderValue JsonMediaTypeHeaderValue = new("application/json");
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
    private static readonly RelativePath Health = Api / "health";
    private static readonly RelativePath Rebalance = Api / "rebalance";

    internal static readonly JsonSerializerOptions SerializerOptions;

    private readonly HttpClient httpClient;
    private readonly Action<HttpRequestMessage>? configureHttpRequestMessage;
    private readonly bool disposeHttpClient;
    private readonly AuthenticationHeaderValue basicAuthHeader;

    static ManagementClient()
    {
        var namingPolicy = new JsonSnakeCaseNamingPolicy();
        SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = namingPolicy,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(namingPolicy),
                new HaParamsConverter()
            }
        };
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
        Action<HttpHandler>? configureHttpHandler = null
    ) : this(
        LegacyEndpointBuilder.Build(hostUrl, portNumber, ssl),
        username,
        password,
        timeout,
        configureHttpRequestMessage,
        configureHttpHandler
    )
    {
    }

    public ManagementClient(
        Uri endpoint,
        string username,
        string password,
        TimeSpan? timeout = null,
        Action<HttpRequestMessage>? configureHttpRequestMessage = null,
        Action<HttpHandler>? configureHttpHandler = null
    )
    {
        if (!endpoint.IsAbsoluteUri) throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, "Endpoint should be absolute");

        this.configureHttpRequestMessage = configureHttpRequestMessage;

        var httpHandler = new HttpHandler();
        configureHttpHandler?.Invoke(httpHandler);
        basicAuthHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
        httpClient = new HttpClient(httpHandler) { Timeout = timeout ?? DefaultTimeout, BaseAddress = endpoint };
        disposeHttpClient = true;
    }

    public ManagementClient(HttpClient httpClient, string username, string password)
    {
        if (httpClient.BaseAddress == null)
            throw new ArgumentNullException(nameof(httpClient.BaseAddress), "Endpoint should be specified");

        if (!httpClient.BaseAddress.IsAbsoluteUri)
            throw new ArgumentOutOfRangeException(nameof(httpClient.BaseAddress), httpClient.BaseAddress, "Endpoint should be absolute");

        this.httpClient = httpClient;
        basicAuthHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
        configureHttpRequestMessage = null;
        disposeHttpClient = false;
    }

    public Uri Endpoint => httpClient.BaseAddress!;

    public Task<Overview> GetOverviewAsync(
        LengthsCriteria? lengthsCriteria = null,
        RatesCriteria? ratesCriteria = null,
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

    public Task<IReadOnlyList<Connection>> GetConnectionsAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Connection>>(Vhosts / vhostName / "connections", cancellationToken);
    }

    public Task CloseConnectionAsync(string connectionName, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(Connections / connectionName, cancellationToken);
    }

    public Task<IReadOnlyList<Channel>> GetChannelsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Channel>>(Channels, cancellationToken);
    }

    public Task<IReadOnlyList<Channel>> GetChannelsAsync(string connectionName, CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Channel>>(Connections / connectionName / "channels", cancellationToken);
    }

    public Task<Channel> GetChannelAsync(
        string channelName,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<Channel>(Channels / channelName, ratesCriteria?.ToQueryParameters(), cancellationToken);
    }

    public Task<IReadOnlyList<Exchange>> GetExchangesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Exchange>>(Exchanges, cancellationToken);
    }

    public Task<PageResult<Exchange>> GetExchangesByPageAsync(PageCriteria pageCriteria, CancellationToken cancellationToken = default)
    {
        return GetAsync<PageResult<Exchange>>(Exchanges, pageCriteria.ToQueryParameters(), cancellationToken);
    }

    public Task<IReadOnlyList<Exchange>> GetExchangesAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Exchange>>(Exchanges / vhostName, cancellationToken);
    }

    public Task<PageResult<Exchange>> GetExchangesByPageAsync(
        string vhostName,
        PageCriteria pageCriteria,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<PageResult<Exchange>>(Exchanges / vhostName, pageCriteria.ToQueryParameters(), cancellationToken);
    }

    public Task<Exchange> GetExchangeAsync(
        string vhostName,
        string exchangeName,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<Exchange>(
            Exchanges / vhostName / exchangeName,
            ratesCriteria?.ToQueryParameters(),
            cancellationToken
        );
    }

    public Task<Queue> GetQueueAsync(
        string vhostName,
        string queueName,
        LengthsCriteria? lengthsCriteria = null,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryParameters = MergeQueryParameters(
            lengthsCriteria?.ToQueryParameters(), ratesCriteria?.ToQueryParameters()
        );
        return GetAsync<Queue>(
            Queues / vhostName / queueName,
            queryParameters,
            cancellationToken
        );
    }

    public Task CreateExchangeAsync(
        string vhostName,
        ExchangeInfo exchangeInfo,
        CancellationToken cancellationToken = default
    )
    {
        return PutAsync(
            Exchanges / vhostName / exchangeInfo.Name,
            exchangeInfo,
            cancellationToken
        );
    }

    public Task DeleteExchangeAsync(string vhostName, string exchangeName, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            Exchanges / vhostName / exchangeName,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsWithSourceAsync(
        string vhostName,
        string exchangeName,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<IReadOnlyList<Binding>>(
            Exchanges / vhostName / exchangeName / "bindings" / "source",
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsWithDestinationAsync(
        string vhostName,
        string exchangeName,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<IReadOnlyList<Binding>>(
            Exchanges / vhostName / exchangeName / "bindings" / "destination",
            cancellationToken
        );
    }

    public Task<PublishResult> PublishAsync(
        string vhostName,
        string exchangeName,
        PublishInfo publishInfo,
        CancellationToken cancellationToken = default
    )
    {
        return PostAsync<PublishInfo, PublishResult>(
            Exchanges / vhostName / exchangeName / "publish",
            publishInfo,
            cancellationToken
        );
    }


    public Task<IReadOnlyList<Queue>> GetQueuesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Queue>>(Queues, cancellationToken);
    }

    public Task<PageResult<Queue>> GetQueuesByPageAsync(PageCriteria pageCriteria, CancellationToken cancellationToken = default)
    {
        return GetAsync<PageResult<Queue>>(Queues, pageCriteria.ToQueryParameters(), cancellationToken);
    }

    public Task<IReadOnlyList<Queue>> GetQueuesAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Queue>>(Queues / vhostName, cancellationToken);
    }

    public Task<PageResult<Queue>> GetQueuesByPageAsync(string vhostName, PageCriteria pageCriteria, CancellationToken cancellationToken = default)
    {
        return GetAsync<PageResult<Queue>>(Queues / vhostName, pageCriteria.ToQueryParameters(), cancellationToken);
    }

    public Task CreateQueueAsync(
        string vhostName,
        QueueInfo queueInfo,
        CancellationToken cancellationToken = default
    )
    {
        return PutAsync(
            Queues / vhostName / queueInfo.Name,
            queueInfo,
            cancellationToken
        );
    }

    public Task DeleteQueueAsync(string vhostName, string queueName, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            Queues / vhostName / queueName, cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsForQueueAsync(
        string vhostName, string queueName, CancellationToken cancellationToken = default
    )
    {
        return GetAsync<IReadOnlyList<Binding>>(
            Queues / vhostName / queueName / "bindings", cancellationToken
        );
    }

    public Task PurgeAsync(string vhostName, string queueName, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            Queues / vhostName / queueName / "contents", cancellationToken
        );
    }

    public Task<IReadOnlyList<Message>> GetMessagesFromQueueAsync(
        string vhostName,
        string queueName,
        GetMessagesFromQueueInfo getMessagesFromQueueInfo,
        CancellationToken cancellationToken = default
    )
    {
        return PostAsync<GetMessagesFromQueueInfo, IReadOnlyList<Message>>(
            Queues / vhostName / queueName / "get",
            getMessagesFromQueueInfo,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetBindingsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Binding>>(Bindings, cancellationToken);
    }

    public Task<IReadOnlyList<Binding>> GetBindingsAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Binding>>(Bindings / vhostName, cancellationToken);
    }

    public Task CreateQueueBindingAsync(
        string vhostName,
        string exchangeName,
        string queueName,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    )
    {
        return PostAsync(
            Bindings / vhostName / "e" / exchangeName / "q" / queueName,
            bindingInfo,
            cancellationToken
        );
    }

    public Task CreateExchangeBindingAsync(
        string vhostName,
        string sourceExchangeName,
        string destinationExchangeName,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    )
    {
        return PostAsync(
            Bindings / vhostName / "e" / sourceExchangeName / "e" / destinationExchangeName,
            bindingInfo,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetQueueBindingsAsync(
        string vhostName,
        string exchangeName,
        string queueName,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<IReadOnlyList<Binding>>(
            Bindings / vhostName / "e" / exchangeName / "q" / queueName,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<Binding>> GetExchangeBindingsAsync(
        string vhostName,
        string fromExchangeName,
        string toExchangeName,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<IReadOnlyList<Binding>>(
            Bindings / vhostName / "e" / fromExchangeName / "e" / toExchangeName,
            cancellationToken
        );
    }

    public Task DeleteQueueBindingAsync(
        string vhostName,
        string exchangeName,
        string queueName,
        string propertiesKey,
        CancellationToken cancellationToken = default
    )
    {
        return DeleteAsync(
            Bindings / vhostName / "e" / exchangeName / "q" / queueName / propertiesKey,
            cancellationToken
        );
    }

    public Task DeleteExchangeBindingAsync(
        string vhostName,
        string fromExchangeName,
        string toExchangeName,
        string propertiesKey,
        CancellationToken cancellationToken = default
    )
    {
        return DeleteAsync(
            Bindings / vhostName / "e" / fromExchangeName / "e" / toExchangeName / propertiesKey,
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
        return PutAsync(Vhosts / vhostName, cancellationToken);
    }

    public Task DeleteVhostAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(Vhosts / vhostName, cancellationToken);
    }

    public Task EnableTracingAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return PutAsync(Vhosts / vhostName, new { Tracing = true }, cancellationToken);
    }

    public Task DisableTracingAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return PutAsync(Vhosts / vhostName, new { Tracing = false }, cancellationToken);
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

    public Task CreatePolicyAsync(
        string vhostName,
        PolicyInfo policyInfo,
        CancellationToken cancellationToken = default
    )
    {
        return PutAsync(Policies / vhostName / policyInfo.Name, policyInfo, cancellationToken);
    }

    public Task DeletePolicyAsync(string vhostName, string policyName, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(Policies / vhostName / policyName, cancellationToken);
    }

    public Task<Parameter> GetParameterAsync(
        string vhostName,
        string componentName,
        string parameterName,
        CancellationToken cancellationToken = default
    )
    {
        return GetAsync<Parameter>(Parameters / componentName / vhostName / parameterName, cancellationToken);
    }

    public Task<IReadOnlyList<Parameter>> GetParametersAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Parameter>>(Parameters, cancellationToken);
    }

    public Task CreateParameterAsync(
        string componentName,
        string vhostName,
        string parameterName,
        object parameterValue,
        CancellationToken cancellationToken = default
    )
    {
        return PutAsync(
            Parameters / componentName / vhostName / parameterName,
            new { Value = parameterValue },
            cancellationToken
        );
    }

    public Task DeleteParameterAsync(
        string componentName,
        string vhostName,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        return DeleteAsync(Parameters / componentName / vhostName / name, cancellationToken);
    }

    public Task CreateUserAsync(UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        return PutAsync(Users / userInfo.Name, userInfo, cancellationToken);
    }

    public Task DeleteUserAsync(string userName, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(Users / userName, cancellationToken);
    }

    public Task<IReadOnlyList<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Permission>>(Permissions, cancellationToken);
    }

    public Task CreatePermissionAsync(string vhostName, PermissionInfo permissionInfo, CancellationToken cancellationToken = default)
    {
        return PutAsync(
            Permissions / vhostName / permissionInfo.UserName,
            permissionInfo,
            cancellationToken
        );
    }

    public Task DeletePermissionAsync(string vhostName, string userName, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(Permissions / vhostName / userName, cancellationToken);
    }

    public Task<IReadOnlyList<TopicPermission>> GetTopicPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<TopicPermission>>(TopicPermissions, cancellationToken);
    }

    public Task CreateTopicPermissionAsync(
        string vhostName,
        TopicPermissionInfo topicPermissionInfo,
        CancellationToken cancellationToken = default
    )
    {
        return PutAsync(
            TopicPermissions / vhostName / topicPermissionInfo.UserName,
            topicPermissionInfo,
            cancellationToken
        );
    }

    public Task DeleteTopicPermissionAsync(string vhostName, string userName, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(TopicPermissions / vhostName / userName, cancellationToken);
    }

    public Task<IReadOnlyList<Federation>> GetFederationsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Federation>>(FederationLinks, cancellationToken);
    }

    public Task<bool> IsAliveAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return GetAsync(AlivenessTest / vhostName, c => c == HttpStatusCode.OK, c => c == HttpStatusCode.ServiceUnavailable, cancellationToken);
    }

    public Task<IReadOnlyList<Consumer>> GetConsumersAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Consumer>>(Consumers, cancellationToken);
    }

    public Task<IReadOnlyList<Consumer>> GetConsumersAsync(string vhostName, CancellationToken cancellationToken = default)
    {
        return GetAsync<IReadOnlyList<Consumer>>(Consumers / vhostName, cancellationToken);
    }

    public Task<bool> HaveAnyClusterAlarmsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync(
            Health / "checks" / "alarms",
            c => c == HttpStatusCode.ServiceUnavailable,
            c => c == HttpStatusCode.OK,
            cancellationToken
        );
    }

    public Task<bool> HaveAnyLocalAlarmsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync(
            Health / "checks" / "local-alarms",
            c => c == HttpStatusCode.ServiceUnavailable,
            c => c == HttpStatusCode.OK,
            cancellationToken
        );
    }

    public Task<bool> HaveAnyClassicQueuesWithoutSynchronisedMirrorsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync(
            Health / "checks" / "node-is-mirror-sync-critical",
            c => c == HttpStatusCode.ServiceUnavailable,
            c => c == HttpStatusCode.OK,
            cancellationToken
        );
    }

    public Task<bool> HaveAnyQuorumQueuesInCriticalStateAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync(
            Health / "checks" / "node-is-quorum-critical",
            c => c == HttpStatusCode.ServiceUnavailable,
            c => c == HttpStatusCode.OK,
            cancellationToken
        );
    }

    public Task RebalanceQueuesAsync(CancellationToken cancellationToken = default)
    {
        return PostAsync(Rebalance / "queues", cancellationToken);
    }

    public void Dispose()
    {
        if (disposeHttpClient)
            httpClient.Dispose();
    }

    private async Task<bool> GetAsync(
        RelativePath path,
        Func<HttpStatusCode, bool> trueCondition,
        Func<HttpStatusCode, bool> falseCondition,
        CancellationToken cancellationToken = default
    )
    {
        using var request = CreateRequest(HttpMethod.Get, path);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        if (trueCondition(response.StatusCode))
            return true;

        if (falseCondition(response.StatusCode))
            return false;

        throw new UnexpectedHttpStatusCodeException(response.StatusCode);
    }

    private Task<T> GetAsync<T>(RelativePath path, CancellationToken cancellationToken = default) => GetAsync<T>(path, null, cancellationToken);

    private async Task<T> GetAsync<T>(
        RelativePath path,
        IReadOnlyDictionary<string, string>? queryParameters,
        CancellationToken cancellationToken = default
    )
    {
        using var request = CreateRequest(HttpMethod.Get, path, queryParameters);
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode == HttpStatusCode.OK);

        return (await response.Content.ReadFromJsonAsync<T>(SerializerOptions, cancellationToken).ConfigureAwait(false))!;
    }

    private async Task<TResult> PostAsync<TItem, TResult>(
        RelativePath path,
        TItem item,
        CancellationToken cancellationToken = default
    )
    {
        using var requestContent = JsonContent.Create(item, JsonMediaTypeHeaderValue, SerializerOptions);
        using var request = CreateRequest(HttpMethod.Post, path, null, requestContent);
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode is HttpStatusCode.OK or HttpStatusCode.Created or HttpStatusCode.NoContent);

        return (await response.Content.ReadFromJsonAsync<TResult>(SerializerOptions, cancellationToken).ConfigureAwait(false))!;
    }

    private async Task PostAsync<TItem>(
        RelativePath path,
        TItem item,
        CancellationToken cancellationToken = default
    )
    {
        using var requestContent = JsonContent.Create(item, JsonMediaTypeHeaderValue, SerializerOptions);
        using var request = CreateRequest(HttpMethod.Post, path, null, requestContent);
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode is HttpStatusCode.OK or HttpStatusCode.Created or HttpStatusCode.NoContent);
    }

    private async Task PostAsync(
        RelativePath path,
        CancellationToken cancellationToken = default
    )
    {
        using var request = CreateRequest(HttpMethod.Post, path);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode is HttpStatusCode.OK or HttpStatusCode.Created or HttpStatusCode.NoContent);
    }

    private async Task DeleteAsync(RelativePath path, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Delete, path);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode == HttpStatusCode.NoContent);
    }

    private async Task PutAsync<T>(
        RelativePath path,
        T item,
        CancellationToken cancellationToken = default
    )
    {
        using var requestContent = JsonContent.Create(item, JsonMediaTypeHeaderValue, SerializerOptions);
        using var request = CreateRequest(HttpMethod.Put, path, null, requestContent);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode is HttpStatusCode.Created or HttpStatusCode.NoContent);
    }

    private async Task PutAsync(RelativePath path, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Put, path);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode is HttpStatusCode.Created or HttpStatusCode.NoContent);
    }

    private HttpRequestMessage CreateRequest(
        HttpMethod httpMethod,
        in RelativePath path,
        IReadOnlyDictionary<string, string>? queryParameters = null,
        HttpContent? content = null
    )
    {
        var request = new HttpRequestMessage(httpMethod, QueryStringHelpers.AddQueryString(path.Build(), queryParameters));
        request.Headers.Authorization = basicAuthHeader;
        request.Content = content;
        configureHttpRequestMessage?.Invoke(request);
        return request;
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
