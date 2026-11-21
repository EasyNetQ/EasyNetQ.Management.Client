using EasyNetQ.Management.Client.Model;
using EasyNetQ.Management.Client.Serialization;
using Refit;

namespace EasyNetQ.Management.Client;

/// <summary>
///     The RabbitMQ management HTTP API. <see cref="ManagementClient"/> implements it; Refit generates the requests.
/// </summary>
[Headers("Authorization: Basic")]
public interface IManagementClient : IDisposable
{
    /// <summary>
    ///     Various random bits of information that describe the whole system.
    /// </summary>
    /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    [Get("/api/overview")]
    Task<Overview> GetOverviewAsync(
        [QueryConverter(typeof(CriteriaQueryConverter))] LengthsCriteria? lengthsCriteria = null,
        [QueryConverter(typeof(CriteriaQueryConverter))] RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of nodes in the RabbitMQ cluster.
    /// </summary>
    [Get("/api/nodes")]
    Task<IReadOnlyList<Node>> GetNodesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     The server definitions - exchanges, queues, bindings, users, virtual hosts, permissions, topic permissions, policies and parameters.
    ///     Everything apart from messages.
    /// </summary>
    [Get("/api/definitions")]
    Task<Definitions> GetDefinitionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all open connections.
    /// </summary>
    [Get("/api/connections")]
    Task<IReadOnlyList<Connection>> GetConnectionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all open connections on the specified VHost.
    /// </summary>
    [Get("/api/vhosts/{vhostName}/connections")]
    Task<IReadOnlyList<Connection>> GetConnectionsAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all open channels.
    /// </summary>
    [Get("/api/channels")]
    Task<IReadOnlyList<Channel>> GetChannelsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all open channels for the given connection.
    /// </summary>
    [Get("/api/connections/{connectionName}/channels")]
    Task<IReadOnlyList<Channel>> GetChannelsAsync(string connectionName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the channel. This returns more detail, including consumers than the GetChannels method.
    /// </summary>
    [Get("/api/channels/{channelName}")]
    Task<Channel> GetChannelAsync(
        string channelName,
        [QueryConverter(typeof(CriteriaQueryConverter))] RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all exchanges.
    /// </summary>
    [Get("/api/exchanges")]
    Task<IReadOnlyList<Exchange>> GetExchangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of exchanges for a page.
    /// </summary>
    [Get("/api/exchanges")]
    Task<PageResult<Exchange>> GetExchangesByPageAsync(
        [QueryConverter(typeof(CriteriaQueryConverter))] PageCriteria pageCriteria,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all exchanges for a virtual host.
    /// </summary>
    [Get("/api/exchanges/{vhostName}")]
    Task<IReadOnlyList<Exchange>> GetExchangesAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of exchanges for a page for a virtual host.
    /// </summary>
    [Get("/api/exchanges/{vhostName}")]
    Task<PageResult<Exchange>> GetExchangesByPageAsync(
        string vhostName,
        [QueryConverter(typeof(CriteriaQueryConverter))] PageCriteria pageCriteria,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all queues.
    /// </summary>
    [Get("/api/queues")]
    Task<IReadOnlyList<Queue>> GetQueuesAsync(
        [QueryConverter(typeof(CriteriaQueryConverter))] StatsCriteria? statsCriteria = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of queues for a page.
    /// </summary>
    [Get("/api/queues")]
    Task<PageResult<Queue>> GetQueuesByPageAsync(
        [QueryConverter(typeof(CriteriaQueryConverter))] PageCriteria pageCriteria,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all queues for a virtual host.
    /// </summary>
    [Get("/api/queues/{vhostName}")]
    Task<IReadOnlyList<Queue>> GetQueuesAsync(
        string vhostName,
        [QueryConverter(typeof(CriteriaQueryConverter))] StatsCriteria? statsCriteria = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of queues for a page for a virtual host.
    /// </summary>
    [Get("/api/queues/{vhostName}")]
    Task<PageResult<Queue>> GetQueuesByPageAsync(
        string vhostName,
        [QueryConverter(typeof(CriteriaQueryConverter))] PageCriteria pageCriteria,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all bindings.
    /// </summary>
    [Get("/api/bindings")]
    Task<IReadOnlyList<Binding>> GetBindingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all bindings within the specified VHost.
    /// </summary>
    [Get("/api/bindings/{vhostName}")]
    Task<IReadOnlyList<Binding>> GetBindingsAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all vhosts.
    /// </summary>
    [Get("/api/vhosts")]
    Task<IReadOnlyList<Vhost>> GetVhostsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all users.
    /// </summary>
    [Get("/api/users")]
    Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all permissions for all users.
    /// </summary>
    [Get("/api/permissions")]
    Task<IReadOnlyList<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all topic permissions for all users.
    /// </summary>
    [Get("/api/topic-permissions")]
    Task<IReadOnlyList<TopicPermission>> GetTopicPermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all consumers.
    /// </summary>
    [Get("/api/consumers")]
    Task<IReadOnlyList<Consumer>> GetConsumersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all consumers for the specified VHost.
    /// </summary>
    [Get("/api/consumers/{vhostName}")]
    Task<IReadOnlyList<Consumer>> GetConsumersAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Closes the given connection
    /// </summary>
    [Delete("/api/connections/{connectionName}")]
    Task CloseConnectionAsync(string connectionName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates the given exchange
    /// </summary>
    [Put("/api/exchanges/{vhostName}/{exchangeName}")]
    Task CreateExchangeAsync(
        string vhostName,
        string exchangeName,
        [Body] ExchangeInfo exchangeInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete the given exchange
    /// </summary>
    [Delete("/api/exchanges/{vhostName}/{exchangeName}")]
    Task DeleteExchangeAsync(
        string vhostName,
        string exchangeName,
        [QueryConverter(typeof(CriteriaQueryConverter))] DeleteExchangeCriteria? deleteExchangeCriteria = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all bindings in which a given exchange is the source.
    /// </summary>
    [Get("/api/exchanges/{vhostName}/{exchangeName}/bindings/source")]
    Task<IReadOnlyList<Binding>> GetBindingsWithSourceAsync(string vhostName, string exchangeName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all bindings in which a given exchange is the destination.
    /// </summary>
    [Get("/api/exchanges/{vhostName}/{exchangeName}/bindings/destination")]
    Task<IReadOnlyList<Binding>> GetBindingsWithDestinationAsync(string vhostName, string exchangeName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Publish a message to a given exchange.
    ///     Please note that the publish / get paths in the HTTP API are intended for injecting
    ///     test messages, diagnostics etc - they do not implement reliable delivery and so should
    ///     be treated as a sysadmin's tool rather than a general API for messaging.
    /// </summary>
    [Post("/api/exchanges/{vhostName}/{exchangeName}/publish")]
    Task<PublishResult> PublishAsync(
        string vhostName,
        string exchangeName,
        [Body] PublishInfo publishInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create the given queue
    /// </summary>
    [Put("/api/queues/{vhostName}/{queueName}")]
    Task CreateQueueAsync(
        string vhostName,
        string queueName,
        [Body] QueueInfo queueInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete the given queue
    /// </summary>
    [Delete("/api/queues/{vhostName}/{queueName}")]
    Task DeleteQueueAsync(
        string vhostName,
        string queueName,
        [QueryConverter(typeof(CriteriaQueryConverter))] DeleteQueueCriteria? deleteQueueCriteria = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all bindings on a given queue.
    /// </summary>
    [Get("/api/queues/{vhostName}/{queueName}/bindings")]
    Task<IReadOnlyList<Binding>> GetBindingsForQueueAsync(string vhostName, string queueName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Purge a queue of all messages
    /// </summary>
    [Delete("/api/queues/{vhostName}/{queueName}/contents")]
    Task PurgeAsync(string vhostName, string queueName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get messages from a queue.
    ///     Please note that the publish / get paths in the HTTP API are intended for
    ///     injecting test messages, diagnostics etc - they do not implement reliable
    ///     delivery and so should be treated as a sysadmin's tool rather than a
    ///     general API for messaging.
    /// </summary>
    [Post("/api/queues/{vhostName}/{queueName}/get")]
    Task<IReadOnlyList<Message>> GetMessagesFromQueueAsync(
        string vhostName,
        string queueName,
        [Body] GetMessagesFromQueueInfo getMessagesFromQueueInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a binding between an exchange and a queue
    /// </summary>
    [Post("/api/bindings/{vhostName}/e/{exchangeName}/q/{queueName}")]
    Task CreateQueueBindingAsync(
        string vhostName,
        string exchangeName,
        string queueName,
        [Body] BindingInfo bindingInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a binding between an exchange and an exchange
    /// </summary>
    [Post("/api/bindings/{vhostName}/e/{sourceExchangeName}/e/{destinationExchangeName}")]
    Task CreateExchangeBindingAsync(
        string vhostName,
        string sourceExchangeName,
        string destinationExchangeName,
        [Body] BindingInfo bindingInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all bindings between an exchange and a queue.
    ///     Remember, an exchange and a queue can be bound together many times!
    /// </summary>
    [Get("/api/bindings/{vhostName}/e/{exchangeName}/q/{queueName}")]
    Task<IReadOnlyList<Binding>> GetQueueBindingsAsync(
        string vhostName,
        string exchangeName,
        string queueName,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all bindings between an exchange and an exchange.
    ///     Remember, an exchange and a exchange can be bound together many times!
    /// </summary>
    [Get("/api/bindings/{vhostName}/e/{sourceExchangeName}/e/{destinationExchangeName}")]
    Task<IReadOnlyList<Binding>> GetExchangeBindingsAsync(
        string vhostName,
        string sourceExchangeName,
        string destinationExchangeName,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete the given binding
    /// </summary>
    [Delete("/api/bindings/{vhostName}/e/{exchangeName}/q/{queueName}/{propertiesKey}")]
    Task DeleteQueueBindingAsync(
        string vhostName,
        string exchangeName,
        string queueName,
        string propertiesKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete the given binding
    /// </summary>
    [Delete("/api/bindings/{vhostName}/e/{sourceExchangeName}/e/{destinationExchangeName}/{propertiesKey}")]
    Task DeleteExchangeBindingAsync(
        string vhostName,
        string sourceExchangeName,
        string destinationExchangeName,
        string propertiesKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a new virtual host
    /// </summary>
    [Put("/api/vhosts/{vhostName}")]
    Task CreateVhostAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a new virtual host, or change an existing one's description, tags, default queue type or tracing
    /// </summary>
    [Put("/api/vhosts/{vhostName}")]
    Task CreateVhostAsync(string vhostName, [Body] VhostInfo vhostInfo, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete a virtual host
    /// </summary>
    [Delete("/api/vhosts/{vhostName}")]
    Task DeleteVhostAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a new user
    /// </summary>
    [Put("/api/users/{userName}")]
    Task CreateUserAsync(string userName, [Body] UserInfo userInfo, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete a user
    /// </summary>
    [Delete("/api/users/{userName}")]
    Task DeleteUserAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a permission
    /// </summary>
    [Put("/api/permissions/{vhostName}/{userName}")]
    Task CreatePermissionAsync(
        string vhostName,
        string userName,
        [Body] PermissionInfo permissionInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete a permission
    /// </summary>
    [Delete("/api/permissions/{vhostName}/{userName}")]
    Task DeletePermissionAsync(string vhostName, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a topic permission
    /// </summary>
    [Put("/api/topic-permissions/{vhostName}/{userName}")]
    Task CreateTopicPermissionAsync(
        string vhostName,
        string userName,
        [Body] TopicPermissionInfo topicPermissionInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete a topic permission
    /// </summary>
    [Delete("/api/topic-permissions/{vhostName}/{userName}")]
    Task DeleteTopicPermissionAsync(string vhostName, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     The raw aliveness test of a virtual host; <see cref="IManagementClientExtensions.IsAliveAsync(IManagementClient, string, CancellationToken)"/> reads it.
    /// </summary>
    [Get("/api/aliveness-test/{vhostName}")]
    Task<IApiResponse> CheckAlivenessAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get an individual exchange by name
    /// </summary>
    [Get("/api/exchanges/{vhostName}/{exchangeName}")]
    Task<Exchange> GetExchangeAsync(
        string vhostName,
        string exchangeName,
        [QueryConverter(typeof(CriteriaQueryConverter))] RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get an individual queue by name
    /// </summary>
    [Get("/api/queues/{vhostName}/{queueName}")]
    Task<Queue> GetQueueAsync(
        string vhostName,
        string queueName,
        [QueryConverter(typeof(CriteriaQueryConverter))] LengthsCriteria? lengthsCriteria = null,
        [QueryConverter(typeof(CriteriaQueryConverter))] RatesCriteria? ratesCriteria = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get an individual vhost by name
    /// </summary>
    [Get("/api/vhosts/{vhostName}")]
    Task<Vhost> GetVhostAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get a user by name
    /// </summary>
    [Get("/api/users/{userName}")]
    Task<User> GetUserAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get collection of Policies on the cluster
    /// </summary>
    [Get("/api/policies")]
    Task<IReadOnlyList<Policy>> GetPoliciesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a policy on the cluster
    /// </summary>
    [Put("/api/policies/{vhostName}/{policyName}")]
    Task CreatePolicyAsync(
        string vhostName,
        string policyName,
        [Body] PolicyInfo policyInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete a policy from the cluster
    /// </summary>
    [Delete("/api/policies/{vhostName}/{policyName}")]
    Task DeletePolicyAsync(string vhostName, string policyName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get specific parameters on the cluster
    /// </summary>
    [Get("/api/parameters/{componentName}/{vhostName}/{parameterName}")]
    Task<Parameter> GetParameterAsync(
        string vhostName,
        string componentName,
        string parameterName,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get all parameters on the cluster
    /// </summary>
    [Get("/api/parameters")]
    Task<IReadOnlyList<Parameter>> GetParametersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get all parameters for a given component on the cluster
    /// </summary>
    [Get("/api/parameters/{componentName}")]
    Task<IReadOnlyList<Parameter>> GetParametersAsync(string componentName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get all parameters for a given component and virtual host on the cluster
    /// </summary>
    [Get("/api/parameters/{componentName}/{vhostName}")]
    Task<IReadOnlyList<Parameter>> GetParametersAsync(string componentName, string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a parameter on the cluster
    /// </summary>
    [Put("/api/parameters/{componentName}/{vhostName}/{parameterName}")]
    Task CreateParameterAsync(
        string componentName,
        string vhostName,
        string parameterName,
        [Body] ParameterValue parameterValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete a parameter from the cluster
    /// </summary>
    [Delete("/api/parameters/{componentName}/{vhostName}/{parameterName}")]
    Task DeleteParameterAsync(
        string componentName,
        string vhostName,
        string parameterName,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get list of federations
    /// </summary>
    [Get("/api/federation-links")]
    Task<IReadOnlyList<Federation>> GetFederationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     The raw cluster alarms health check; <see cref="IManagementClientExtensions.HaveAnyClusterAlarmsAsync"/> reads it.
    /// </summary>
    [Get("/api/health/checks/alarms")]
    Task<IApiResponse> CheckAlarmsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     The raw local alarms health check; <see cref="IManagementClientExtensions.HaveAnyLocalAlarmsAsync"/> reads it.
    /// </summary>
    [Get("/api/health/checks/local-alarms")]
    Task<IApiResponse> CheckLocalAlarmsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     The raw quorum-critical health check; <see cref="IManagementClientExtensions.HaveAnyQuorumQueuesInCriticalStateAsync"/> reads it.
    /// </summary>
    [Get("/api/health/checks/node-is-quorum-critical")]
    Task<IApiResponse> CheckQuorumCriticalAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Rebalances all queues in all vhosts. This operation is asynchronous therefore please check
    ///     the RabbitMQ log file for messages regarding the success or failure of the operation.
    /// </summary>
    [Post("/api/rebalance/queues")]
    Task RebalanceQueuesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all shovel statuses.
    /// </summary>
    [Get("/api/shovels")]
    Task<IReadOnlyList<ShovelStatus>> GetShovelStatusesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all shovel statuses for a virtual host.
    /// </summary>
    [Get("/api/shovels/{vhostName}")]
    Task<IReadOnlyList<ShovelStatus>> GetShovelStatusesAsync(string vhostName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of all per-user limits.
    /// </summary>
    [Get("/api/user-limits")]
    Task<IReadOnlyList<UserLimits>> GetUserLimitsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     A list of per-user limits for a specific user.
    /// </summary>
    [Get("/api/user-limits/{userName}")]
    Task<IReadOnlyList<UserLimits>> GetUserLimitsAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sets per-user max connections limit for user.
    /// </summary>
    [Put("/api/user-limits/{userName}/max-connections")]
    Task CreateUserMaxConnectionsLimitAsync(string userName, [Body] LimitInfo limitInfo, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sets per-user max channels limit for user.
    /// </summary>
    [Put("/api/user-limits/{userName}/max-channels")]
    Task CreateUserMaxChannelsLimitAsync(string userName, [Body] LimitInfo limitInfo, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes per-user max connections limit for user.
    /// </summary>
    [Delete("/api/user-limits/{userName}/max-connections")]
    Task DeleteUserMaxConnectionsLimitAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes per-user max channels limit for user.
    /// </summary>
    [Delete("/api/user-limits/{userName}/max-channels")]
    Task DeleteUserMaxChannelsLimitAsync(string userName, CancellationToken cancellationToken = default);
}
