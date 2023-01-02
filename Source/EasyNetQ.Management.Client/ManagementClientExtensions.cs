using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client;

public static class ManagementClientExtensions
{
    /// <summary>
    ///     Various random bits of information that describe the whole system.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Overview GetOverview(
        this IManagementClient client,
        LengthsCriteria? lengthsCriteria = null,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetOverviewAsync(lengthsCriteria, ratesCriteria, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }


    /// <summary>
    ///     A list of nodes in the RabbitMQ cluster.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Node> GetNodes(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetNodesAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     The server definitions - exchanges, queues, bindings, users, virtual hosts, permissions.
    ///     Everything apart from messages.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Definitions GetDefinitions(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetDefinitionsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all open connections.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Connection> GetConnections(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetConnectionsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all open channels.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Channel> GetChannels(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetChannelsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all open channels.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="connection"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Channel>> GetChannelsAsync(
        this IManagementClient client,
        Connection connection,
        CancellationToken cancellationToken = default
    ) => client.GetChannelsAsync(connection.Name, cancellationToken);

    /// <summary>
    ///     Gets the channel. This returns more detail, including consumers than the GetChannels method.
    /// </summary>
    /// <param name="client"></param>
    /// <returns>The channel.</returns>
    /// <param name="channelName">Channel name.</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    public static Channel GetChannel(
        this IManagementClient client,
        string channelName,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetChannelAsync(channelName, ratesCriteria, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all exchanges.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Exchange> GetExchanges(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetExchangesAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all queues.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Queue> GetQueues(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetQueuesAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all queues for a virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Queue>> GetQueuesAsync(
        this IManagementClient client,
        Vhost vhost,
        CancellationToken cancellationToken = default
    ) => client.GetQueuesAsync(vhost.Name, cancellationToken);

    /// <summary>
    ///     A list of all queues for a virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Queue> GetQueues(
        this IManagementClient client,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetQueuesAsync(vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all queues.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="pageCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static PageResult<Queue> GetQueuesByPage(
        this IManagementClient client,
        PageCriteria pageCriteria,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetQueuesByPageAsync(pageCriteria, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all queues for a virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost"></param>
    /// <param name="pageCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<PageResult<Queue>> GetQueuesByPageAsync(
        this IManagementClient client,
        Vhost vhost,
        PageCriteria pageCriteria,
        CancellationToken cancellationToken = default
    ) => client.GetQueuesByPageAsync(vhost.Name, pageCriteria, cancellationToken);

    /// <summary>
    ///     A list of all queues for a virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost"></param>
    /// <param name="pageCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static PageResult<Queue> GetQueuesByPage(
        this IManagementClient client,
        Vhost vhost,
        PageCriteria pageCriteria,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetQueuesByPageAsync(vhost, pageCriteria, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all bindings.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Binding> GetBindings(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetBindingsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all vhosts.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Vhost> GetVhosts(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetVhostsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all users.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<User> GetUsers(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetUsersAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all permissions for all users.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Permission> GetPermissions(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetPermissionsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all topic permissions for all users.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<TopicPermission> GetTopicPermissions(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetTopicPermissionsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Closes the given connection
    /// </summary>
    /// <param name="client"></param>
    /// <param name="connection"></param>
    /// <param name="cancellationToken"></param>
    public static Task CloseConnectionAsync(
        this IManagementClient client,
        Connection connection,
        CancellationToken cancellationToken = default
    ) => client.CloseConnectionAsync(connection.Name, cancellationToken);

    /// <summary>
    ///     Closes the given connection
    /// </summary>
    /// <param name="client"></param>
    /// <param name="connection"></param>
    /// <param name="cancellationToken"></param>
    public static void CloseConnection(
        this IManagementClient client,
        Connection connection,
        CancellationToken cancellationToken = default
    )
    {
        client.CloseConnectionAsync(connection, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Closes the given connection
    /// </summary>
    /// <param name="client"></param>
    /// <param name="connectionName"></param>
    /// <param name="cancellationToken"></param>
    public static void CloseConnection(
        this IManagementClient client,
        string connectionName,
        CancellationToken cancellationToken = default
    )
    {
        client.CloseConnectionAsync(connectionName, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Creates the given exchange
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchangeInfo"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    public static Task CreateExchangeAsync(
        this IManagementClient client,
        ExchangeInfo exchangeInfo,
        Vhost vhost,
        CancellationToken cancellationToken = default
    ) => client.CreateExchangeAsync(vhost.Name, exchangeInfo, cancellationToken);

    /// <summary>
    ///     Creates the given exchange
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchangeInfo"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    public static void CreateExchange(
        this IManagementClient client,
        ExchangeInfo exchangeInfo,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        client.CreateExchangeAsync(exchangeInfo, vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete the given exchange
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteExchangeAsync(
        this IManagementClient client,
        Exchange exchange,
        CancellationToken cancellationToken = default
    ) => client.DeleteExchangeAsync(exchange.Vhost, exchange.Name, cancellationToken);

    /// <summary>
    ///     Delete the given exchange
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange"></param>
    /// <param name="cancellationToken"></param>
    public static void DeleteExchange(
        this IManagementClient client,
        Exchange exchange,
        CancellationToken cancellationToken = default
    )
    {
        client.DeleteExchangeAsync(exchange, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all bindings in which a given exchange is the client.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Binding>> GetBindingsWithSourceAsync(
        this IManagementClient client,
        Exchange exchange,
        CancellationToken cancellationToken = default
    ) => client.GetBindingsWithSourceAsync(exchange.Vhost, exchange.Name, cancellationToken);

    /// <summary>
    ///     A list of all bindings in which a given exchange is the client.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Binding> GetBindingsWithSource(
        this IManagementClient client,
        Exchange exchange,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetBindingsWithSourceAsync(exchange, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all bindings in which a given exchange is the destination.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Binding>> GetBindingsWithDestinationAsync(
        this IManagementClient client,
        Exchange exchange,
        CancellationToken cancellationToken = default
    ) => client.GetBindingsWithDestinationAsync(exchange.Vhost, exchange.Name, cancellationToken);

    /// <summary>
    ///     A list of all bindings in which a given exchange is the destination.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Binding> GetBindingsWithDestination(
        this IManagementClient client,
        Exchange exchange,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetBindingsWithDestinationAsync(exchange, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Publish a message to a given exchange.
    ///     Please note that the publish / get paths in the HTTP API are intended for injecting
    ///     test messages, diagnostics etc - they do not implement reliable delivery and so should
    ///     be treated as a sysadmin's tool rather than a general API for messaging.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange">The exchange</param>
    /// <param name="publishInfo">The publication parameters</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A PublishResult, routed == true if the message was sent to at least one queue</returns>
    public static Task<PublishResult> PublishAsync(
        this IManagementClient client,
        Exchange exchange,
        PublishInfo publishInfo,
        CancellationToken cancellationToken = default
    ) => client.PublishAsync(exchange.Vhost, exchange.Name, publishInfo, cancellationToken);

    /// <summary>
    ///     Publish a message to a given exchange.
    ///     Please note that the publish / get paths in the HTTP API are intended for injecting
    ///     test messages, diagnostics etc - they do not implement reliable delivery and so should
    ///     be treated as a sysadmin's tool rather than a general API for messaging.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange">The exchange</param>
    /// <param name="publishInfo">The publication parameters</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A PublishResult, routed == true if the message was sent to at least one queue</returns>
    public static PublishResult Publish(
        this IManagementClient client,
        Exchange exchange,
        PublishInfo publishInfo,
        CancellationToken cancellationToken = default
    )
    {
        return client.PublishAsync(exchange, publishInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create the given queue
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queueInfo"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    public static Task CreateQueueAsync(
        this IManagementClient client,
        QueueInfo queueInfo,
        Vhost vhost,
        CancellationToken cancellationToken = default
    ) => client.CreateQueueAsync(vhost.Name, queueInfo, cancellationToken);

    /// <summary>
    ///     Create the given queue
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queueInfo"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    public static void CreateQueue(
        this IManagementClient client,
        QueueInfo queueInfo,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        client.CreateQueueAsync(queueInfo, vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete the given queue
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteQueueAsync(
        this IManagementClient client,
        Queue queue,
        CancellationToken cancellationToken = default
    ) => client.DeleteQueueAsync(queue.Vhost, queue.Name, cancellationToken);

    /// <summary>
    ///     Delete the given queue
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    public static void DeleteQueue(
        this IManagementClient client,
        Queue queue,
        CancellationToken cancellationToken = default
    )
    {
        client.DeleteQueueAsync(queue, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all bindings on a given queue.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    public static Task<IReadOnlyList<Binding>> GetBindingsForQueueAsync(
        this IManagementClient client,
        Queue queue,
        CancellationToken cancellationToken = default
    ) => client.GetBindingsForQueueAsync(queue.Vhost, queue.Name, cancellationToken);

    /// <summary>
    ///     A list of all bindings on a given queue.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    public static IReadOnlyList<Binding> GetBindingsForQueue(
        this IManagementClient client,
        Queue queue,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetBindingsForQueueAsync(queue, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Purge a queue of all messages
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    public static Task PurgeAsync(
        this IManagementClient client,
        Queue queue,
        CancellationToken cancellationToken = default
    ) => client.PurgeAsync(queue.Vhost, queue.Name, cancellationToken);

    /// <summary>
    ///     Purge a queue of all messages
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    public static void Purge(
        this IManagementClient client,
        Queue queue,
        CancellationToken cancellationToken = default
    )
    {
        client.PurgeAsync(queue, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get messages from a queue.
    ///     Please note that the publish / get paths in the HTTP API are intended for
    ///     injecting test messages, diagnostics etc - they do not implement reliable
    ///     delivery and so should be treated as a sysadmin's tool rather than a
    ///     general API for messaging.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queue">The queue to retrieve from</param>
    /// <param name="getMessagesFromQueueInfo">The criteria for the retrieve</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Messages</returns>
    public static Task<IReadOnlyList<Message>> GetMessagesFromQueueAsync(
        this IManagementClient client,
        Queue queue,
        GetMessagesFromQueueInfo getMessagesFromQueueInfo,
        CancellationToken cancellationToken = default
    ) => client.GetMessagesFromQueueAsync(queue.Vhost, queue.Name, getMessagesFromQueueInfo, cancellationToken);

    /// <summary>
    ///     Get messages from a queue.
    ///     Please note that the publish / get paths in the HTTP API are intended for
    ///     injecting test messages, diagnostics etc - they do not implement reliable
    ///     delivery and so should be treated as a sysadmin's tool rather than a
    ///     general API for messaging.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queue">The queue to retrieve from</param>
    /// <param name="getMessagesFromQueueInfo">The criteria for the retrieve</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Messages</returns>
    public static IReadOnlyList<Message> GetMessagesFromQueue(
        this IManagementClient client,
        Queue queue,
        GetMessagesFromQueueInfo getMessagesFromQueueInfo,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetMessagesFromQueueAsync(queue, getMessagesFromQueueInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create a binding between an exchange and a queue
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange">the exchange</param>
    /// <param name="queue">the queue</param>
    /// <param name="bindingInfo">properties of the binding</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The binding that was created</returns>
    public static Task CreateQueueBindingAsync(
        this IManagementClient client,
        Exchange exchange,
        Queue queue,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    ) => client.CreateQueueBindingAsync(exchange.Vhost, exchange.Name, queue.Name, bindingInfo, cancellationToken);

    /// <summary>
    ///     Create a binding between an exchange and a queue
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange">the exchange</param>
    /// <param name="queue">the queue</param>
    /// <param name="bindingInfo">properties of the binding</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The binding that was created</returns>
    public static void CreateQueueBinding(
        this IManagementClient client,
        Exchange exchange,
        Queue queue,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    )
    {
        client.CreateQueueBindingAsync(exchange, queue, bindingInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }


    /// <summary>
    ///     Create a binding between an exchange and an exchange
    /// </summary>
    /// <param name="client"></param>
    /// <param name="sourceExchange">the source exchange</param>
    /// <param name="destinationExchange">the destination exchange</param>
    /// <param name="bindingInfo">properties of the binding</param>
    /// <param name="cancellationToken"></param>
    public static Task CreateExchangeBindingAsync(
        this IManagementClient client,
        Exchange sourceExchange,
        Exchange destinationExchange,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    ) => client.CreateExchangeBindingAsync(sourceExchange.Vhost, sourceExchange.Name, destinationExchange.Name, bindingInfo, cancellationToken);

    /// <summary>
    ///     Create a binding between an exchange and an exchange
    /// </summary>
    /// <param name="client"></param>
    /// <param name="sourceExchange">the source exchange</param>
    /// <param name="destinationExchange">the destination exchange</param>
    /// <param name="bindingInfo">properties of the binding</param>
    /// <param name="cancellationToken"></param>
    public static void CreateExchangeBinding(
        this IManagementClient client,
        Exchange sourceExchange,
        Exchange destinationExchange,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    )
    {
        client.CreateExchangeBindingAsync(sourceExchange, destinationExchange, bindingInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all bindings between an exchange and a queue.
    ///     Remember, an exchange and a queue can be bound together many times!
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Binding>> GetQueueBindingsAsync(
        this IManagementClient client,
        Exchange exchange,
        Queue queue,
        CancellationToken cancellationToken = default
    ) => client.GetQueueBindingsAsync(exchange.Vhost, exchange.Name, queue.Name, cancellationToken);

    /// <summary>
    ///     A list of all bindings between an exchange and a queue.
    ///     Remember, an exchange and a queue can be bound together many times!
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchange"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Binding> GetQueueBindings(
        this IManagementClient client,
        Exchange exchange,
        Queue queue,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetQueueBindingsAsync(exchange, queue, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }


    /// <summary>
    ///     A list of all bindings between an exchange and a queue.
    ///     Remember, an exchange and a queue can be bound together many times!
    /// </summary>
    /// <param name="client"></param>
    /// <param name="fromExchange"></param>
    /// <param name="toExchange"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IReadOnlyList<Binding>> GetExchangeBindingsAsync(
        this IManagementClient client,
        Exchange fromExchange,
        Exchange toExchange,
        CancellationToken cancellationToken = default
    ) => client.GetExchangeBindingsAsync(fromExchange.Vhost, fromExchange.Name, toExchange.Name, cancellationToken);

    /// <summary>
    ///     A list of all bindings between an exchange and an exchange.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="fromExchange"></param>
    /// <param name="toExchange"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Binding> GetExchangeBindings(
        this IManagementClient client,
        Exchange fromExchange,
        Exchange toExchange,
        CancellationToken cancellationToken = default
    )
    {
        return GetExchangeBindingsAsync(client, fromExchange, toExchange, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete the given binding
    /// </summary>
    /// <param name="client"></param>
    /// <param name="binding"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteBindingAsync(
        this IManagementClient client,
        Binding binding,
        CancellationToken cancellationToken = default
    )
    {
        return binding.DestinationType switch
        {
            "queue" => client.DeleteQueueBindingAsync(binding.Vhost, binding.Source, binding.Destination, binding.PropertiesKey!, cancellationToken),
            "exchange" => client.DeleteExchangeBindingAsync(binding.Vhost, binding.Source, binding.Destination, binding.PropertiesKey!, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(binding.DestinationType), binding.DestinationType, null)
        };
    }

    /// <summary>
    ///     Delete the given binding
    /// </summary>
    /// <param name="client"></param>
    /// <param name="binding"></param>
    /// <param name="cancellationToken"></param>
    public static void DeleteBinding(
        this IManagementClient client,
        Binding binding,
        CancellationToken cancellationToken = default
    )
    {
        client.DeleteBindingAsync(binding, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create a new virtual host
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">The name of the new virtual host</param>
    /// <param name="cancellationToken"></param>
    public static void CreateVhost(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    )
    {
        client.CreateVhostAsync(vhostName, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a virtual host
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost">The virtual host to delete</param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteVhostAsync(
        this IManagementClient client,
        Vhost vhost,
        CancellationToken cancellationToken = default
    ) => client.DeleteVhostAsync(vhost.Name, cancellationToken);

    /// <summary>
    ///     Delete a virtual host
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost">The virtual host to delete</param>
    /// <param name="cancellationToken"></param>
    public static void DeleteVhost(
        this IManagementClient client,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        client.DeleteVhostAsync(vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Enable tracing on given virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost">The virtual host on which to enable tracing</param>
    /// <param name="cancellationToken"></param>
    public static async Task<Vhost> EnableTracingAsync(
        this IManagementClient client,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        await client.EnableTracingAsync(vhost.Name, cancellationToken).ConfigureAwait(false);
        return vhost with { Tracing = true };
    }

    /// <summary>
    ///     Enable tracing on given virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost">The virtual host on which to enable tracing</param>
    /// <param name="cancellationToken"></param>
    public static void EnableTracing(
        this IManagementClient client,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        client.EnableTracingAsync(vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Disables tracing on given virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost">The virtual host on which to disable tracing</param>
    /// <param name="cancellationToken"></param>
    public static async Task<Vhost> DisableTracingAsync(
        this IManagementClient client,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        await client.DisableTracingAsync(vhost.Name, cancellationToken).ConfigureAwait(false);
        return vhost with { Tracing = false };
    }

    /// <summary>
    ///     Disables tracing on given virtual host.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost">The virtual host on which to disable tracing</param>
    /// <param name="cancellationToken"></param>
    public static void DisableTracing(
        this IManagementClient client,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        client.DisableTracingAsync(vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create a new user
    /// </summary>
    /// <param name="client"></param>
    /// <param name="userInfo">The user to create</param>
    /// <param name="cancellationToken"></param>
    public static void CreateUser(
        this IManagementClient client,
        UserInfo userInfo,
        CancellationToken cancellationToken = default
    )
    {
        client.CreateUserAsync(userInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a user
    /// </summary>
    /// <param name="client"></param>
    /// <param name="user">The user to delete</param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteUserAsync(
        this IManagementClient client,
        User user,
        CancellationToken cancellationToken = default
    ) => client.DeleteUserAsync(user.Name, cancellationToken);

    /// <summary>
    ///     Delete a user
    /// </summary>
    /// <param name="client"></param>
    /// <param name="user">The user to delete</param>
    /// <param name="cancellationToken"></param>
    public static void DeleteUser(
        this IManagementClient client,
        User user,
        CancellationToken cancellationToken = default
    )
    {
        client.DeleteUserAsync(user, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create a permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost"></param>
    /// <param name="permissionInfo"></param>
    /// <param name="cancellationToken"></param>
    public static Task CreatePermissionAsync(
        this IManagementClient client,
        Vhost vhost,
        PermissionInfo permissionInfo,
        CancellationToken cancellationToken = default
    ) => client.CreatePermissionAsync(vhost.Name, permissionInfo, cancellationToken);

    /// <summary>
    ///     Create a permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost"></param>
    /// <param name="permissionInfo"></param>
    /// <param name="cancellationToken"></param>
    public static void CreatePermission(
        this IManagementClient client,
        Vhost vhost,
        PermissionInfo permissionInfo,
        CancellationToken cancellationToken = default
    )
    {
        client.CreatePermissionAsync(vhost, permissionInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="permission"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeletePermissionAsync(
        this IManagementClient client,
        Permission permission,
        CancellationToken cancellationToken = default
    ) => client.DeletePermissionAsync(permission.Vhost, permission.User, cancellationToken);

    /// <summary>
    ///     Delete a permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="permission">The permission to delete</param>
    /// <param name="cancellationToken"></param>
    public static void DeletePermission(
        this IManagementClient client,
        Permission permission,
        CancellationToken cancellationToken = default
    )
    {
        client.DeletePermissionAsync(permission, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create a topic permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="topicPermissionInfo"></param>
    /// <param name="cancellationToken"></param>
    public static void CreateTopicPermission(
        this IManagementClient client,
        string vhostName,
        TopicPermissionInfo topicPermissionInfo,
        CancellationToken cancellationToken = default
    )
    {
        client.CreateTopicPermissionAsync(vhostName, topicPermissionInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a topic permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="permission">The topic permission to delete</param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteTopicPermissionAsync(
        this IManagementClient client,
        TopicPermission permission,
        CancellationToken cancellationToken = default
    ) => client.DeleteTopicPermissionAsync(permission.Vhost, permission.User, cancellationToken);

    /// <summary>
    ///     Delete a topic permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="permission">The topic permission to delete</param>
    /// <param name="cancellationToken"></param>
    public static void DeleteTopicPermission(
        this IManagementClient client,
        TopicPermission permission,
        CancellationToken cancellationToken = default
    )
    {
        client.DeleteTopicPermissionAsync(permission, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Update the password of an user.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="userName">The name of a user</param>
    /// <param name="newPassword">The new password to set</param>
    /// <param name="cancellationToken"></param>
    public static void ChangeUserPassword(
        this IManagementClient client,
        string userName,
        string newPassword,
        CancellationToken cancellationToken = default
    )
    {
        client.ChangeUserPasswordAsync(userName, newPassword, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Declares a test queue, then publishes and consumes a message. Intended for use
    ///     by monitoring tools. If everything is working correctly, will return true.
    ///     Note: the test queue will not be deleted (to to prevent queue churn if this
    ///     is repeatedly pinged).
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    public static Task<bool> IsAliveAsync(
        this IManagementClient client,
        Vhost vhost,
        CancellationToken cancellationToken = default
    ) => client.IsAliveAsync(vhost.Name, cancellationToken);

    /// <summary>
    ///     Declares a test queue, then publishes and consumes a message. Intended for use
    ///     by monitoring tools. If everything is working correctly, will return true.
    ///     Note: the test queue will not be deleted (to prevent queue churn if this
    ///     is repeatedly pinged).
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    public static bool IsAlive(
        this IManagementClient client,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        return client.IsAliveAsync(vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get an individual exchange by name
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost">The virtual host that contains the exchange</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The exchange</returns>
    public static Task<Exchange> GetExchangeAsync(
        this IManagementClient client,
        Vhost vhost,
        string exchangeName,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    ) => client.GetExchangeAsync(vhost.Name, exchangeName, ratesCriteria, cancellationToken);

    /// <summary>
    ///     Get an individual exchange by name
    /// </summary>
    /// <param name="client"></param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="vhost">The virtual host that contains the exchange</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The exchange</returns>
    public static Exchange GetExchange(
        this IManagementClient client,
        string exchangeName,
        Vhost vhost,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetExchangeAsync(vhost, exchangeName, ratesCriteria, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get an individual queue by name
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queueName">The name of the queue</param>
    /// <param name="vhost">The virtual host that contains the queue</param>
    /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The Queue</returns>
    public static Task<Queue> GetQueueAsync(
        this IManagementClient client,
        Vhost vhost,
        string queueName,
        LengthsCriteria? lengthsCriteria = null,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    ) => client.GetQueueAsync(vhost.Name, queueName, lengthsCriteria, ratesCriteria, cancellationToken);

    /// <summary>
    ///     Get an individual queue by name
    /// </summary>
    /// <param name="client"></param>
    /// <param name="queueName">The name of the queue</param>
    /// <param name="vhost">The virtual host that contains the queue</param>
    /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The Queue</returns>
    public static Queue GetQueue(
        this IManagementClient client,
        Vhost vhost,
        string queueName,
        LengthsCriteria? lengthsCriteria = null,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetQueueAsync(vhost, queueName, lengthsCriteria, ratesCriteria, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get an individual vhost by name
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName">The VHost</param>
    /// <param name="cancellationToken"></param>
    public static Vhost GetVhost(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetVhostAsync(vhostName, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get a user by name
    /// </summary>
    /// <param name="client"></param>
    /// <param name="userName">The name of the user</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The User</returns>
    public static User GetUser(
        this IManagementClient client,
        string userName,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetUserAsync(userName, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get collection of Policies on the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Policies</returns>
    public static IReadOnlyList<Policy> GetPolicies(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetPoliciesAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Creates a policy on the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="policy">Policy to create</param>
    /// <param name="cancellationToken"></param>
    public static Task CreatePolicyAsync(
        this IManagementClient client,
        Policy policy,
        CancellationToken cancellationToken = default
    ) => client.CreatePolicyAsync(
        policy.Vhost,
        new PolicyInfo
        {
            Name = policy.Name,
            ApplyTo = policy.ApplyTo,
            Definition = policy.Definition,
            Priority = policy.Priority,
            Pattern = policy.Pattern
        },
        cancellationToken
    );

    /// <summary>
    ///     Creates a policy on the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="policy">Policy to create</param>
    /// <param name="cancellationToken"></param>
    public static void CreatePolicy(
        this IManagementClient client,
        Policy policy,
        CancellationToken cancellationToken = default
    )
    {
        client.CreatePolicyAsync(policy, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a policy from the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhost">vhost on which the policy resides</param>
    /// <param name="policyName">Policy name</param>
    /// <param name="cancellationToken"></param>
    public static Task DeletePolicyAsync(
        this IManagementClient client,
        Vhost vhost,
        string policyName,
        CancellationToken cancellationToken = default
    ) => client.DeletePolicyAsync(vhost.Name, policyName, cancellationToken);

    /// <summary>
    ///     Delete a policy from the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="policyName">Policy name</param>
    /// <param name="vhost">vhost on which the policy resides</param>
    /// <param name="cancellationToken"></param>
    public static void DeletePolicy(
        this IManagementClient client,
        Vhost vhost,
        string policyName,
        CancellationToken cancellationToken = default
    )
    {
        client.DeletePolicyAsync(vhost, policyName, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get all parameters on the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    public static IReadOnlyList<Parameter> GetParameters(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetParametersAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Creates a parameter on the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="parameter">Parameter to create</param>
    /// <param name="cancellationToken"></param>
    public static Task CreateParameterAsync(
        this IManagementClient client,
        Parameter parameter,
        CancellationToken cancellationToken = default
    ) => client.CreateParameterAsync(parameter.Component, parameter.Vhost, parameter.Name, parameter.Value, cancellationToken);

    /// <summary>
    ///     Creates a parameter on the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="parameter">Parameter to create</param>
    /// <param name="cancellationToken"></param>
    public static void CreateParameter(
        this IManagementClient client,
        Parameter parameter,
        CancellationToken cancellationToken = default
    )
    {
        client.CreateParameterAsync(parameter, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a parameter from the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="componentName"></param>
    /// <param name="vhostName"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    public static void DeleteParameter(
        this IManagementClient client,
        string componentName,
        string vhostName,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        client.DeleteParameterAsync(componentName, vhostName, name, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get list of federations
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Federation> GetFederations(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.GetFederationsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Update the password of an user.
    /// </summary>
    /// <param name="client">The client</param>
    /// <param name="userName">The name of a user</param>
    /// <param name="newPassword">The new password to set</param>
    /// <param name="cancellationToken"></param>
    public static async Task ChangeUserPasswordAsync(
        this IManagementClient client,
        string userName,
        string newPassword,
        CancellationToken cancellationToken = default
    )
    {
        var user = await client.GetUserAsync(userName, cancellationToken).ConfigureAwait(false);

        var userInfo = new UserInfo(userName, newPassword, null, user.Tags);

        await client.CreateUserAsync(userInfo, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Returns true if there are any alarms in effect in the cluster
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static bool HaveAnyClusterAlarms(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.HaveAnyClusterAlarmsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Returns true if there are any alarms in effect in on a target node
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static bool HaveAnyLocalAlarms(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.HaveAnyLocalAlarmsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }


    /// <summary>
    ///     Returns true if there are classic mirrored queues without synchronised mirrors online (queues that would potentially lose data if the target node is shut down).
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static bool HaveAnyClassicQueuesWithoutSynchronisedMirrorsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.HaveAnyClassicQueuesWithoutSynchronisedMirrorsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Returns true if there are quorum queues with minimum online quorum (queues that would lose their quorum and availability if the target node is shut down)
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static bool HaveAnyQuorumQueuesInCriticalStateAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        return client.HaveAnyQuorumQueuesInCriticalStateAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Rebalances all queues in all vhosts. This operation is asynchronous therefore please check the RabbitMQ log file for messages regarding the success or failure of the operation.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static void RebalanceQueues(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    )
    {
        client.RebalanceQueuesAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }
}
