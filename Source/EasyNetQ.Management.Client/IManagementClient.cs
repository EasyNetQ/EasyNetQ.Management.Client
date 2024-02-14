using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client;

public interface IManagementClient : IDisposable
{
    /// <summary>
    ///     The endpoint that the client is using.
    /// </summary>
    Uri Endpoint { get; }

    Task<TResult> GetAsync<TResult>(
        RelativePath path,
        IEnumerable<KeyValuePair<string, string>>? queryParameters,
        CancellationToken cancellationToken = default);

    Task<bool> CheckAsync(
        RelativePath path,
        CancellationToken cancellationToken = default);

    Task PutAsync<TBody>(
        RelativePath path,
        TBody item,
        CancellationToken cancellationToken = default);
    Task PutAsync(
        RelativePath path,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        RelativePath path,
        CancellationToken cancellationToken = default);

    Task<TResult> PostAsync<TBody, TResult>(
        RelativePath path,
        TBody item,
        CancellationToken cancellationToken = default);
    Task PostAsync<TBody>(
        RelativePath path,
        TBody item,
        CancellationToken cancellationToken = default);
    Task PostAsync(
        RelativePath path,
        CancellationToken cancellationToken = default);
}

public static partial class IManagementClientExtensions
{
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
    private static readonly RelativePath ShovelStatuses = Api / "shovels";

    private static IEnumerable<KeyValuePair<string, string>>? ConcatNullableQueryParameters(params IEnumerable<KeyValuePair<string, string>>?[] multipleQueryParameters)
    {
        return multipleQueryParameters.All(qp => qp == null) ? null : ConcatQueryParameters(multipleQueryParameters);
    }

    private static IEnumerable<KeyValuePair<string, string>> ConcatQueryParameters(params IEnumerable<KeyValuePair<string, string>>?[] multipleQueryParameters)
    {
        foreach (var queryParameters in multipleQueryParameters)
        {
            if (queryParameters == null)
                continue;
            foreach (var queryParameter in queryParameters)
                yield return queryParameter;
        }
    }

    public static Task<TResult> GetAsync<TResult>(
        this IManagementClient client,
        RelativePath path,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetAsync<TResult>(path, null, cancellationToken);
    }

    /// <summary>
    ///     Various random bits of information that describe the whole system.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<Overview> GetOverviewAsync(
        this IManagementClient client,
        LengthsCriteria? lengthsCriteria = null,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryParameters =
            ConcatNullableQueryParameters(lengthsCriteria?.QueryParameters, ratesCriteria?.QueryParameters);
        return client.GetAsync<Overview>(Overview, queryParameters, cancellationToken);
    }

    /// <summary>
    ///     A list of nodes in the RabbitMQ cluster.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Node>> GetNodesAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Node>>(Nodes, cancellationToken);

    /// <summary>
    ///     The server definitions - exchanges, queues, bindings, users, virtual hosts, permissions, topic permissions, policies and parameters.
    ///     Everything apart from messages.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<Definitions> GetDefinitionsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<Definitions>(Definitions, cancellationToken);

    /// <summary>
    ///     A list of all open connections.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Connection>> GetConnectionsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Connection>>(Connections, cancellationToken);

    /// <summary>
    ///     A list of all open connections on the specified VHost.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Connection>> GetConnectionsAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Connection>>(Vhosts / vhostName / "connections", cancellationToken);

    /// <summary>
    ///     A list of all open channels.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Channel>> GetChannelsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Channel>>(Channels, cancellationToken);

    /// <summary>
    ///     A list of all open channels for the given connection.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="connectionName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Channel>> GetChannelsAsync(
        this IManagementClient client,
        string connectionName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Channel>>(Connections / connectionName / "channels", cancellationToken);

    /// <summary>
    ///     Gets the channel. This returns more detail, including consumers than the GetChannels method.
    /// </summary>
    /// <returns>The channel.</returns>
    /// <param name="client"></param>
    /// <param name="channelName">Channel name.</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    public static Task<Channel> GetChannelAsync(
        this IManagementClient client,
        string channelName,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<Channel>(Channels / channelName, ratesCriteria?.QueryParameters, cancellationToken);

    /// <summary>
    ///     A list of all exchanges.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Exchange>> GetExchangesAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Exchange>>(Exchanges, cancellationToken);

    /// <summary>
    ///     A list of exchanges for a page.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="pageCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<PageResult<Exchange>> GetExchangesByPageAsync(
        this IManagementClient client,
        PageCriteria pageCriteria,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<PageResult<Exchange>>(Exchanges, pageCriteria.QueryParameters, cancellationToken);

    /// <summary>
    ///     A list of all exchanges for a virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Exchange>> GetExchangesAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Exchange>>(Exchanges / vhostName, cancellationToken);

    /// <summary>
    ///     A list of exchanges for a page for a virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="pageCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<PageResult<Exchange>> GetExchangesByPageAsync(
        this IManagementClient client,
        string vhostName,
        PageCriteria pageCriteria,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<PageResult<Exchange>>(Exchanges / vhostName, pageCriteria.QueryParameters, cancellationToken);

    /// <summary>
    ///     A list of all queues.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="statsCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Queue>> GetQueuesAsync(
        this IManagementClient client,
        StatsCriteria? statsCriteria = null,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Queue>>(Queues, statsCriteria?.QueryParameters, cancellationToken);

    /// <summary>
    ///     A list of queues for a page.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="pageCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<PageResult<Queue>> GetQueuesByPageAsync(
        this IManagementClient client,
        PageCriteria pageCriteria,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<PageResult<Queue>>(Queues, pageCriteria.QueryParameters, cancellationToken);

    /// <summary>
    ///     A list of all queues for a virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="statsCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Queue>> GetQueuesAsync(
        this IManagementClient client,
        string vhostName,
        StatsCriteria? statsCriteria = null,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Queue>>(Queues / vhostName, statsCriteria?.QueryParameters, cancellationToken);

    /// <summary>
    ///     A list of queues for a page for a virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="pageCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<PageResult<Queue>> GetQueuesByPageAsync(
        this IManagementClient client,
        string vhostName,
        PageCriteria pageCriteria,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<PageResult<Queue>>(Queues / vhostName, pageCriteria.QueryParameters, cancellationToken);

    /// <summary>
    ///     A list of all bindings.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Binding>> GetBindingsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Binding>>(Bindings, cancellationToken);

    /// <summary>
    ///     A list of all bindings within the specified VHost.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Binding>> GetBindingsAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Binding>>(Bindings / vhostName, cancellationToken);

    /// <summary>
    ///     A list of all vhosts.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Vhost>> GetVhostsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Vhost>>(Vhosts, cancellationToken);

    /// <summary>
    ///     A list of all users.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<User>> GetUsersAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<User>>(Users, cancellationToken);

    /// <summary>
    ///     A list of all permissions for all users.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Permission>> GetPermissionsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Permission>>(Permissions, cancellationToken);

    /// <summary>
    ///     A list of all topic permissions for all users.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<TopicPermission>> GetTopicPermissionsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<TopicPermission>>(TopicPermissions, cancellationToken);

    /// <summary>
    ///     A list of all consumers.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Consumer>> GetConsumersAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Consumer>>(Consumers, cancellationToken);

    /// <summary>
    ///     A list of all consumers for the specified VHost.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Consumer>> GetConsumersAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Consumer>>(Consumers / vhostName, cancellationToken);

    /// <summary>
    ///     Closes the given connection
    /// </summary>
    /// <param name="client"></param>
    /// <param name="connectionName"></param>
    /// <param name="cancellationToken"></param>
    public static Task CloseConnectionAsync(
        this IManagementClient client,
        string connectionName,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(Connections / connectionName, cancellationToken);

    /// <summary>
    ///     Creates the given exchange
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="exchangeInfo"></param>
    /// <param name="cancellationToken"></param>
    public static Task CreateExchangeAsync(
        this IManagementClient client,
        string vhostName,
        string exchangeName,
        ExchangeInfo exchangeInfo,
        CancellationToken cancellationToken = default
    ) => client.PutAsync(Exchanges / vhostName / exchangeName, exchangeInfo, cancellationToken);

    /// <summary>
    ///     Delete the given exchange
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteExchangeAsync(
        this IManagementClient client,
        string vhostName,
        string exchangeName,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(Exchanges / vhostName / exchangeName, cancellationToken);

    /// <summary>
    ///     A list of all bindings in which a given exchange is the source.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Binding>> GetBindingsWithSourceAsync(
        this IManagementClient client,
        string vhostName,
        string exchangeName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Binding>>(Exchanges / vhostName / exchangeName / "bindings" / "source", cancellationToken);

    /// <summary>
    ///     A list of all bindings in which a given exchange is the destination.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Binding>> GetBindingsWithDestinationAsync(
        this IManagementClient client,
        string vhostName,
        string exchangeName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Binding>>(Exchanges / vhostName / exchangeName / "bindings" / "destination", cancellationToken);

    /// <summary>
    ///     Publish a message to a given exchange.
    ///     Please note that the publish / get paths in the HTTP API are intended for injecting
    ///     test messages, diagnostics etc - they do not implement reliable delivery and so should
    ///     be treated as a sysadmin's tool rather than a general API for messaging.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">The vhost</param>
    /// <param name="exchangeName">The exchange</param>
    /// <param name="publishInfo">The publication parameters</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A PublishResult, routed == true if the message was sent to at least one queue</returns>
    public static Task<PublishResult> PublishAsync(
        this IManagementClient client,
        string vhostName,
        string exchangeName,
        PublishInfo publishInfo,
        CancellationToken cancellationToken = default
    ) => client.PostAsync<PublishInfo, PublishResult>(Exchanges / vhostName / exchangeName / "publish", publishInfo, cancellationToken);

    /// <summary>
    ///     Create the given queue
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="queueName"></param>
    /// <param name="queueInfo"></param>
    /// <param name="cancellationToken"></param>
    public static Task CreateQueueAsync(
        this IManagementClient client,
        string vhostName,
        string queueName,
        QueueInfo queueInfo,
        CancellationToken cancellationToken = default
    ) => client.PutAsync(Queues / vhostName / queueName, queueInfo, cancellationToken);

    /// <summary>
    ///     Delete the given queue
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="queueName"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteQueueAsync(
        this IManagementClient client,
        string vhostName,
        string queueName,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(Queues / vhostName / queueName, cancellationToken);

    /// <summary>
    ///     A list of all bindings on a given queue.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="queueName"></param>
    /// <param name="cancellationToken"></param>
    public static Task<IReadOnlyList<Binding>> GetBindingsForQueueAsync(
        this IManagementClient client,
        string vhostName,
        string queueName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Binding>>(Queues / vhostName / queueName / "bindings", cancellationToken);

    /// <summary>
    ///     Purge a queue of all messages
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="queueName"></param>
    /// <param name="cancellationToken"></param>
    public static Task PurgeAsync(
        this IManagementClient client,
        string vhostName,
        string queueName,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(Queues / vhostName / queueName / "contents", cancellationToken);

    /// <summary>
    ///     Get messages from a queue.
    ///     Please note that the publish / get paths in the HTTP API are intended for
    ///     injecting test messages, diagnostics etc - they do not implement reliable
    ///     delivery and so should be treated as a sysadmin's tool rather than a
    ///     general API for messaging.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="queueName">The queue to retrieve from</param>
    /// <param name="getMessagesFromQueueInfo">The criteria for the retrieve</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Messages</returns>
    public static Task<IReadOnlyList<Message>> GetMessagesFromQueueAsync(
        this IManagementClient client,
        string vhostName,
        string queueName,
        GetMessagesFromQueueInfo getMessagesFromQueueInfo,
        CancellationToken cancellationToken = default
    ) => client.PostAsync<GetMessagesFromQueueInfo, IReadOnlyList<Message>>(Queues / vhostName / queueName / "get", getMessagesFromQueueInfo, cancellationToken);

    /// <summary>
    ///     Create a binding between an exchange and a queue
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">the vhost</param>
    /// <param name="exchangeName">the exchange</param>
    /// <param name="queueName">the queue</param>
    /// <param name="bindingInfo">properties of the binding</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The binding that was created</returns>
    public static Task CreateQueueBindingAsync(
        this IManagementClient client,
        string vhostName,
        string exchangeName,
        string queueName,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    ) => client.PostAsync(Bindings / vhostName / "e" / exchangeName / "q" / queueName, bindingInfo, cancellationToken);

    /// <summary>
    ///     Create a binding between an exchange and an exchange
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">the vhost</param>
    /// <param name="sourceExchangeName">the source exchange</param>
    /// <param name="destinationExchangeName">the destination exchange</param>
    /// <param name="bindingInfo">properties of the binding</param>
    /// <param name="cancellationToken"></param>
    public static Task CreateExchangeBindingAsync(
        this IManagementClient client,
        string vhostName,
        string sourceExchangeName,
        string destinationExchangeName,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    ) => client.PostAsync(Bindings / vhostName / "e" / sourceExchangeName / "e" / destinationExchangeName, bindingInfo, cancellationToken);

    /// <summary>
    ///     A list of all bindings between an exchange and a queue.
    ///     Remember, an exchange and a queue can be bound together many times!
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="queueName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Binding>> GetQueueBindingsAsync(
        this IManagementClient client,
        string vhostName,
        string exchangeName,
        string queueName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Binding>>(Bindings / vhostName / "e" / exchangeName / "q" / queueName, cancellationToken);

    /// <summary>
    ///     A list of all bindings between an exchange and an exchange.
    ///     Remember, an exchange and a exchange can be bound together many times!
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="sourceExchangeName"></param>
    /// <param name="destinationExchangeName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Binding>> GetExchangeBindingsAsync(
        this IManagementClient client,
        string vhostName,
        string sourceExchangeName,
        string destinationExchangeName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Binding>>(Bindings / vhostName / "e" / sourceExchangeName / "e" / destinationExchangeName, cancellationToken);

    /// <summary>
    ///     Delete the given binding
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="queueName"></param>
    /// <param name="propertiesKey"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteQueueBindingAsync(
        this IManagementClient client,
        string vhostName,
        string exchangeName,
        string queueName,
        string propertiesKey,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(Bindings / vhostName / "e" / exchangeName / "q" / queueName / propertiesKey, cancellationToken);

    /// <summary>
    ///     Delete the given binding
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="sourceExchangeName"></param>
    /// <param name="destinationExchangeName"></param>
    /// <param name="propertiesKey"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteExchangeBindingAsync(
        this IManagementClient client,
        string vhostName,
        string sourceExchangeName,
        string destinationExchangeName,
        string propertiesKey,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(Bindings / vhostName / "e" / sourceExchangeName / "e" / destinationExchangeName / propertiesKey, cancellationToken);

    /// <summary>
    ///     Create a new virtual host
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">The name of the new virtual host</param>
    /// <param name="cancellationToken"></param>
    public static Task CreateVhostAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.PutAsync(Vhosts / vhostName, cancellationToken);

    /// <summary>
    ///     Delete a virtual host
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">The virtual host to delete</param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteVhostAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(Vhosts / vhostName, cancellationToken);

    /// <summary>
    ///     Enable tracing on given virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">The virtual host on which to enable tracing</param>
    /// <param name="cancellationToken"></param>
    public static Task EnableTracingAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.PutAsync(Vhosts / vhostName, new { Tracing = true }, cancellationToken);

    /// <summary>
    ///     Disables tracing on given virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">The virtual host on which to disable tracing</param>
    /// <param name="cancellationToken"></param>
    public static Task DisableTracingAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.PutAsync(Vhosts / vhostName, new { Tracing = false }, cancellationToken);

    /// <summary>
    ///     Create a new user
    /// </summary>
    /// <param name="client"></param>
    /// <param name="userName">The user name to create</param>
    /// <param name="userInfo">The user info to create</param>
    /// <param name="cancellationToken"></param>
    public static Task CreateUserAsync(
        this IManagementClient client,
        string userName,
        UserInfo userInfo,
        CancellationToken cancellationToken = default
    ) => client.PutAsync(Users / userName, userInfo, cancellationToken);

    /// <summary>
    ///     Delete a user
    /// </summary>
    /// <param name="client"></param>
    /// <param name="userName">The user to delete</param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteUserAsync(
        this IManagementClient client,
        string userName,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(Users / userName, cancellationToken);

    /// <summary>
    ///     Create a permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="userName"></param>
    /// <param name="permissionInfo"></param>
    /// <param name="cancellationToken"></param>
    public static Task CreatePermissionAsync(
        this IManagementClient client,
        string vhostName,
        string userName,
        PermissionInfo permissionInfo,
        CancellationToken cancellationToken = default
    ) => client.PutAsync(Permissions / vhostName / userName, permissionInfo, cancellationToken);

    /// <summary>
    ///     Delete a permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeletePermissionAsync(
        this IManagementClient client,
        string vhostName,
        string userName,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(Permissions / vhostName / userName, cancellationToken);

    /// <summary>
    ///     Create a topic permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="userName"></param>
    /// <param name="topicPermissionInfo"></param>
    /// <param name="cancellationToken"></param>
    public static Task CreateTopicPermissionAsync(
        this IManagementClient client,
        string vhostName,
        string userName,
        TopicPermissionInfo topicPermissionInfo,
        CancellationToken cancellationToken = default
    ) => client.PutAsync(TopicPermissions / vhostName / userName, topicPermissionInfo, cancellationToken);

    /// <summary>
    ///     Delete a topic permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteTopicPermissionAsync(
        this IManagementClient client,
        string vhostName,
        string userName,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(TopicPermissions / vhostName / userName, cancellationToken);

    /// <summary>
    ///     Declares a test queue, then publishes and consumes a message. Intended for use
    ///     by monitoring tools. If everything is working correctly, will return true.
    ///     Note: the test queue will not be deleted (to prevent queue churn if this
    ///     is repeatedly pinged).
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="cancellationToken"></param>
    public static Task<bool> IsAliveAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.CheckAsync(AlivenessTest / vhostName, cancellationToken);

    /// <summary>
    ///     Get an individual exchange by name
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">The virtual host that contains the exchange</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The exchange</returns>
    public static Task<Exchange> GetExchangeAsync(
        this IManagementClient client,
        string vhostName,
        string exchangeName,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<Exchange>(Exchanges / vhostName / exchangeName, ratesCriteria?.QueryParameters, cancellationToken);

    /// <summary>
    ///     Get an individual queue by name
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queueName">The name of the queue</param>
    /// <param name="vhostName">The virtual host that contains the queue</param>
    /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The Queue</returns>
    public static Task<Queue> GetQueueAsync(
        this IManagementClient client,
        string vhostName,
        string queueName,
        LengthsCriteria? lengthsCriteria = null,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryParameters =
            ConcatNullableQueryParameters(lengthsCriteria?.QueryParameters, ratesCriteria?.QueryParameters);
        return client.GetAsync<Queue>(Queues / vhostName / queueName, queryParameters, cancellationToken);
    }

    /// <summary>
    ///     Get an individual vhost by name
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">The VHost</param>
    /// <param name="cancellationToken"></param>
    public static Task<Vhost> GetVhostAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<Vhost>(Vhosts / vhostName, cancellationToken);

    /// <summary>
    ///     Get a user by name
    /// </summary>
    /// <param name="client"></param>
    /// <param name="userName">The name of the user</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The User</returns>
    public static Task<User> GetUserAsync(
        this IManagementClient client,
        string userName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<User>(Users / userName, cancellationToken);

    /// <summary>
    ///     Get collection of Policies on the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Policies</returns>
    public static Task<IReadOnlyList<Policy>> GetPoliciesAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Policy>>(Policies, cancellationToken);

    /// <summary>
    ///     Creates a policy on the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="policyName"></param>
    /// <param name="policyInfo"></param>
    /// <param name="cancellationToken"></param>
    public static Task CreatePolicyAsync(
        this IManagementClient client,
        string vhostName,
        string policyName,
        PolicyInfo policyInfo,
        CancellationToken cancellationToken = default
    ) => client.PutAsync(Policies / vhostName / policyName, policyInfo, cancellationToken);

    /// <summary>
    ///     Delete a policy from the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">vhost on which the policy resides</param>
    /// <param name="policyName">Policy name</param>
    /// <param name="cancellationToken"></param>
    public static Task DeletePolicyAsync(
        this IManagementClient client,
        string vhostName,
        string policyName,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(Policies / vhostName / policyName, cancellationToken);

    /// <summary>
    ///     Get specific parameters on the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="componentName"></param>
    /// <param name="parameterName"></param>
    /// <param name="cancellationToken"></param>
    public static Task<Parameter> GetParameterAsync(
        this IManagementClient client,
        string vhostName,
        string componentName,
        string parameterName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<Parameter>(Parameters / componentName / vhostName / parameterName, cancellationToken);

    /// <summary>
    ///     Get all parameters on the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    public static Task<IReadOnlyList<Parameter>> GetParametersAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Parameter>>(Parameters, cancellationToken);

    /// <summary>
    ///     Creates a parameter on the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="componentName"></param>
    /// <param name="vhostName"></param>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    /// <param name="cancellationToken"></param>
    public static Task CreateParameterAsync(
        this IManagementClient client,
        string componentName,
        string vhostName,
        string parameterName,
        object parameterValue,
        CancellationToken cancellationToken = default
    ) => client.PutAsync(Parameters / componentName / vhostName / parameterName, new { Value = parameterValue }, cancellationToken);

    /// <summary>
    ///     Delete a parameter from the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="componentName"></param>
    /// <param name="vhostName"></param>
    /// <param name="parameterName"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteParameterAsync(
        this IManagementClient client,
        string componentName,
        string vhostName,
        string parameterName,
        CancellationToken cancellationToken = default
    ) => client.DeleteAsync(Parameters / componentName / vhostName / parameterName, cancellationToken);

    /// <summary>
    ///     Get list of federations
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Federation>> GetFederationsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<Federation>>(FederationLinks, cancellationToken);

    /// <summary>
    ///     Returns true if there are any alarms in effect in the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<bool> HaveAnyClusterAlarmsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => !await client.CheckAsync(Health / "checks" / "alarms", cancellationToken).ConfigureAwait(false);

    /// <summary>
    ///     Returns true if there are any alarms in effect in on a target node
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<bool> HaveAnyLocalAlarmsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => !await client.CheckAsync(Health / "checks" / "local-alarms", cancellationToken).ConfigureAwait(false);


    /// <summary>
    ///     Returns true if there are classic mirrored queues without synchronised mirrors online (queues that would potentially lose data if the target node is shut down).
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<bool> HaveAnyClassicQueuesWithoutSynchronisedMirrorsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => !await client.CheckAsync(Health / "checks" / "node-is-mirror-sync-critical", cancellationToken).ConfigureAwait(false);

    /// <summary>
    ///     Returns true if there are quorum queues with minimum online quorum (queues that would lose their quorum and availability if the target node is shut down)
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<bool> HaveAnyQuorumQueuesInCriticalStateAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => !await client.CheckAsync(Health / "checks" / "node-is-quorum-critical", cancellationToken).ConfigureAwait(false);

    /// <summary>
    ///     Rebalances all queues in all vhosts. This operation is asynchronous therefore please check the RabbitMQ log file for messages regarding the success or failure of the operation.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RebalanceQueuesAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.PostAsync(Rebalance / "queues", cancellationToken);

    /// <summary>
    ///     A list of all shovel statuses.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<ShovelStatus>> GetShovelStatusesAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<ShovelStatus>>(ShovelStatuses, cancellationToken);

    /// <summary>
    ///     A list of all shovel statuses for a virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<ShovelStatus>> GetShovelStatusesAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.GetAsync<IReadOnlyList<ShovelStatus>>(ShovelStatuses / vhostName, cancellationToken);

    // Current implementation in RabbitMQ has a lot of bugs
    // 
    // /// <summary>
    // ///     Get an individual shovel status by name.
    // /// </summary>
    // /// <param name="vhostName"></param>
    // /// <param name="shovelName"></param>
    // /// <param name="cancellationToken"></param>
    // /// <returns></returns>
    // Task<ShovelStatus> GetShovelStatusAsync(string vhostName, string shovelName, CancellationToken cancellationToken = default);
}
