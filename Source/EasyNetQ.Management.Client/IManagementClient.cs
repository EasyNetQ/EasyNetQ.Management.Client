using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client;

public interface IManagementClient : IDisposable
{
    /// <summary>
    ///     The endpoint that the client is using.
    /// </summary>
    Uri Endpoint { get; }

    /// <summary>
    ///     Various random bits of information that describe the whole system.
    /// </summary>
    /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Overview> GetOverviewAsync(
        LengthsCriteria? lengthsCriteria = null,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     A list of nodes in the RabbitMQ cluster.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Node>> GetNodesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     The server definitions - exchanges, queues, bindings, users, virtual hosts, permissions.
    ///     Everything apart from messages.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Definitions> GetDefinitionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all open connections.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Connection>> GetConnectionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all open channels.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Channel>> GetChannelsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all open channels for the given connection.
    /// </summary>
    /// <param name="connectionName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Channel>> GetChannelsAsync(
        string connectionName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Gets the channel. This returns more detail, including consumers than the GetChannels method.
    /// </summary>
    /// <returns>The channel.</returns>
    /// <param name="channelName">Channel name.</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    Task<Channel> GetChannelAsync(
        string channelName,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     A list of all exchanges.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Exchange>> GetExchangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of exchanges for a page.
    /// </summary>
    /// <param name="pageCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PageResult<Exchange>> GetExchangesByPageAsync(PageCriteria pageCriteria, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all exchanges.
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Exchange>> GetExchangesAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of exchanges for a page for a virtual host.
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="pageCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PageResult<Exchange>> GetExchangesByPageAsync(string vhostName, PageCriteria pageCriteria, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all queues.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Queue>> GetQueuesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of queues for a page.
    /// </summary>
    /// <param name="pageCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PageResult<Queue>> GetQueuesByPageAsync(PageCriteria pageCriteria, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all queues for a virtual host.
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Queue>> GetQueuesAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of queues for a page for a virtual host.
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="pageCriteria"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PageResult<Queue>> GetQueuesByPageAsync(string vhostName, PageCriteria pageCriteria, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all bindings.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Binding>> GetBindingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all vhosts.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Vhost>> GetVhostsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all users.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all permissions for all users.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all topic permissions for all users.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TopicPermission>> GetTopicPermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all consumers.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Consumer>> GetConsumersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Closes the given connection
    /// </summary>
    /// <param name="connectionName"></param>
    /// <param name="cancellationToken"></param>
    Task CloseConnectionAsync(
        string connectionName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Creates the given exchange
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="exchangeInfo"></param>
    /// <param name="cancellationToken"></param>
    Task CreateExchangeAsync(
        string vhostName,
        ExchangeInfo exchangeInfo,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Delete the given exchange
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="cancellationToken"></param>
    Task DeleteExchangeAsync(
        string vhostName,
        string exchangeName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     A list of all bindings in which a given exchange is the source.
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Binding>> GetBindingsWithSourceAsync(
        string vhostName,
        string exchangeName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     A list of all bindings in which a given exchange is the destination.
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Binding>> GetBindingsWithDestinationAsync(
        string vhostName,
        string exchangeName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Publish a message to a given exchange.
    ///     Please note that the publish / get paths in the HTTP API are intended for injecting
    ///     test messages, diagnostics etc - they do not implement reliable delivery and so should
    ///     be treated as a sysadmin's tool rather than a general API for messaging.
    /// </summary>
    /// <param name="vhostName">The vhost</param>
    /// <param name="exchangeName">The exchange</param>
    /// <param name="publishInfo">The publication parameters</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A PublishResult, routed == true if the message was sent to at least one queue</returns>
    Task<PublishResult> PublishAsync(
        string vhostName,
        string exchangeName,
        PublishInfo publishInfo,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Create the given queue
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="queueInfo"></param>
    /// <param name="cancellationToken"></param>
    Task CreateQueueAsync(
        string vhostName,
        QueueInfo queueInfo,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Delete the given queue
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="queueName"></param>
    /// <param name="cancellationToken"></param>
    Task DeleteQueueAsync(
        string vhostName,
        string queueName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     A list of all bindings on a given queue.
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="queueName"></param>
    /// <param name="cancellationToken"></param>
    Task<IReadOnlyList<Binding>> GetBindingsForQueueAsync(
        string vhostName,
        string queueName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Purge a queue of all messages
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="queueName"></param>
    /// <param name="cancellationToken"></param>
    Task PurgeAsync(
        string vhostName,
        string queueName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Get messages from a queue.
    ///     Please note that the publish / get paths in the HTTP API are intended for
    ///     injecting test messages, diagnostics etc - they do not implement reliable
    ///     delivery and so should be treated as a sysadmin's tool rather than a
    ///     general API for messaging.
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="queueName">The queue to retrieve from</param>
    /// <param name="getMessagesFromQueueInfo">The criteria for the retrieve</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Messages</returns>
    Task<IReadOnlyList<Message>> GetMessagesFromQueueAsync(
        string vhostName,
        string queueName,
        GetMessagesFromQueueInfo getMessagesFromQueueInfo,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Create a binding between an exchange and a queue
    /// </summary>
    /// <param name="vhostName">the vhost</param>
    /// <param name="exchangeName">the exchange</param>
    /// <param name="queueName">the queue</param>
    /// <param name="bindingInfo">properties of the binding</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The binding that was created</returns>
    Task CreateQueueBindingAsync(
        string vhostName,
        string exchangeName,
        string queueName,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Create a binding between an exchange and an exchange
    /// </summary>
    /// <param name="vhostName">the vhost</param>
    /// <param name="sourceExchangeName">the source exchange</param>
    /// <param name="destinationExchangeName">the destination exchange</param>
    /// <param name="bindingInfo">properties of the binding</param>
    /// <param name="cancellationToken"></param>
    Task CreateExchangeBindingAsync(
        string vhostName,
        string sourceExchangeName,
        string destinationExchangeName,
        BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     A list of all bindings between an exchange and a queue.
    ///     Remember, an exchange and a queue can be bound together many times!
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="queueName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Binding>> GetQueueBindingsAsync(
        string vhostName,
        string exchangeName,
        string queueName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     A list of all bindings between an exchange and an exchange.
    ///     Remember, an exchange and a exchange can be bound together many times!
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="fromExchangeName"></param>
    /// <param name="toExchangeName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Binding>> GetExchangeBindingsAsync(
        string vhostName,
        string fromExchangeName,
        string toExchangeName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Delete the given binding
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="queueName"></param>
    /// <param name="propertiesKey"></param>
    /// <param name="cancellationToken"></param>
    Task DeleteQueueBindingAsync(
        string vhostName,
        string exchangeName,
        string queueName,
        string propertiesKey,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Delete the given binding
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="fromExchangeName"></param>
    /// <param name="sourceExchangeName"></param>
    /// <param name="propertiesKey"></param>
    /// <param name="cancellationToken"></param>
    Task DeleteExchangeBindingAsync(
        string vhostName,
        string fromExchangeName,
        string sourceExchangeName,
        string propertiesKey,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Create a new virtual host
    /// </summary>
    /// <param name="vhostName">The name of the new virtual host</param>
    /// <param name="cancellationToken"></param>
    Task CreateVhostAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete a virtual host
    /// </summary>
    /// <param name="vhostName">The virtual host to delete</param>
    /// <param name="cancellationToken"></param>
    Task DeleteVhostAsync(
        string vhostName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Enable tracing on given virtual host.
    /// </summary>
    /// <param name="vhostName">The virtual host on which to enable tracing</param>
    /// <param name="cancellationToken"></param>
    Task EnableTracingAsync(
        string vhostName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Disables tracing on given virtual host.
    /// </summary>
    /// <param name="vhostName">The virtual host on which to disable tracing</param>
    /// <param name="cancellationToken"></param>
    Task DisableTracingAsync(
        string vhostName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Create a new user
    /// </summary>
    /// <param name="userInfo">The user to create</param>
    /// <param name="cancellationToken"></param>
    Task CreateUserAsync(
        UserInfo userInfo,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Delete a user
    /// </summary>
    /// <param name="userName">The user to delete</param>
    /// <param name="cancellationToken"></param>
    Task DeleteUserAsync(
        string userName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Create a permission
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="permissionInfo"></param>
    /// <param name="cancellationToken"></param>
    Task CreatePermissionAsync(
        string vhostName,
        PermissionInfo permissionInfo,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Delete a permission
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    Task DeletePermissionAsync(
        string vhostName,
        string userName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Create a topic permission
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="topicPermissionInfo"></param>
    /// <param name="cancellationToken"></param>
    Task CreateTopicPermissionAsync(
        string vhostName,
        TopicPermissionInfo topicPermissionInfo,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Delete a topic permission
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    Task DeleteTopicPermissionAsync(
        string vhostName,
        string userName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Declares a test queue, then publishes and consumes a message. Intended for use
    ///     by monitoring tools. If everything is working correctly, will return true.
    ///     Note: the test queue will not be deleted (to to prevent queue churn if this
    ///     is repeatedly pinged).
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="cancellationToken"></param>
    Task<bool> IsAliveAsync(
        string vhostName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Get an individual exchange by name
    /// </summary>
    /// <param name="vhostName">The virtual host that contains the exchange</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The exchange</returns>
    Task<Exchange> GetExchangeAsync(
        string vhostName,
        string exchangeName,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Get an individual queue by name
    /// </summary>
    /// <param name="queueName">The name of the queue</param>
    /// <param name="vhostName">The virtual host that contains the queue</param>
    /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The Queue</returns>
    Task<Queue> GetQueueAsync(
        string vhostName,
        string queueName,
        LengthsCriteria? lengthsCriteria = null,
        RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Get an individual vhost by name
    /// </summary>
    /// <param name="vhostName">The VHost</param>
    /// <param name="cancellationToken"></param>
    Task<Vhost> GetVhostAsync(
        string vhostName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Get a user by name
    /// </summary>
    /// <param name="userName">The name of the user</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The User</returns>
    Task<User> GetUserAsync(
        string userName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Get collection of Policies on the cluster
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Policies</returns>
    Task<IReadOnlyList<Policy>> GetPoliciesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a policy on the cluster
    /// </summary>
    /// <param name="vhostName"></param>
    /// <param name="policyInfo"></param>
    /// <param name="cancellationToken"></param>
    Task CreatePolicyAsync(
        string vhostName,
        PolicyInfo policyInfo,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Delete a policy from the cluster
    /// </summary>
    /// <param name="vhostName">vhost on which the policy resides</param>
    /// <param name="policyName">Policy name</param>
    /// <param name="cancellationToken"></param>
    Task DeletePolicyAsync(
        string vhostName,
        string policyName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Get all parameters on the cluster
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task<IReadOnlyList<Parameter>> GetParametersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a parameter on the cluster
    /// </summary>
    /// <param name="componentName"></param>
    /// <param name="vhostName"></param>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    /// <param name="cancellationToken"></param>
    Task CreateParameterAsync(
        string componentName,
        string vhostName,
        string parameterName,
        object parameterValue,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Delete a parameter from the cluster
    /// </summary>
    /// <param name="componentName"></param>
    /// <param name="vhostName"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    Task DeleteParameterAsync(
        string componentName,
        string vhostName,
        string name,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Get list of federations
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Federation>> GetFederationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Returns true if there are any alarms in effect in the cluster
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HaveHealthCheckClusterAlarmsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Returns true if there are any alarms in effect in on a target node
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HaveHealthCheckLocalAlarmsAsync(CancellationToken cancellationToken = default);
}
