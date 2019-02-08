using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.Management.Client.Model;
using JetBrains.Annotations;

namespace EasyNetQ.Management.Client
{
    public interface IManagementClient : IDisposable
    {
        /// <summary>
        /// The host URL that this instance is using.
        /// </summary>
        string HostUrl { get; }

        /// <summary>
        /// The Username that this instance is connecting as.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// The port number this instance connects using.
        /// </summary>
        int PortNumber { get; }

        /// <summary>
        /// Various random bits of information that describe the whole system.
        /// </summary>
        /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
        /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Overview> GetOverviewAsync(
            GetLengthsCriteria lengthsCriteria = null,
            GetRatesCriteria ratesCriteria = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of nodes in the RabbitMQ cluster.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Node>> GetNodesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// The server definitions - exchanges, queues, bindings, users, virtual hosts, permissions. 
        /// Everything apart from messages.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Definitions> GetDefinitionsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all open connections.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Connection>> GetConnectionsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all open channels.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Channel>> GetChannelsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the channel. This returns more detail, including consumers than the GetChannels method.
        /// </summary>
        /// <returns>The channel.</returns>
        /// <param name="channelName">Channel name.</param>
        /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
        /// <param name="cancellationToken"></param>
        Task<Channel> GetChannelAsync(
            string channelName,
            GetRatesCriteria ratesCriteria = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all exchanges.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Exchange>> GetExchangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all queues.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Queue>> GetQueuesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all bindings.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Binding>> GetBindingsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all vhosts.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Vhost>> GetVHostsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all users.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all permissions for all users.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all topic permissions for all users.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<TopicPermission>> GetTopicPermissionsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Closes the given connection
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cancellationToken"></param>
        Task CloseConnectionAsync(
            [NotNull] Connection connection,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates the given exchange
        /// </summary>
        /// <param name="exchangeInfo"></param>
        /// <param name="vhost"></param>
        /// <param name="cancellationToken"></param>
        Task<Exchange> CreateExchangeAsync(
            [NotNull] ExchangeInfo exchangeInfo,
            [NotNull] Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete the given exchange
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="cancellationToken"></param>
        Task DeleteExchangeAsync(
            [NotNull] Exchange exchange,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all bindings in which a given exchange is the source.
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Binding>> GetBindingsWithSourceAsync(
            [NotNull] Exchange exchange,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all bindings in which a given exchange is the destination.
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Binding>> GetBindingsWithDestinationAsync(
            [NotNull] Exchange exchange,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Publish a message to a given exchange.
        /// 
        /// Please note that the publish / get paths in the HTTP API are intended for injecting 
        /// test messages, diagnostics etc - they do not implement reliable delivery and so should 
        /// be treated as a sysadmin's tool rather than a general API for messaging.
        /// </summary>
        /// <param name="exchange">The exchange</param>
        /// <param name="publishInfo">The publication parameters</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A PublishResult, routed == true if the message was sent to at least one queue</returns>
        Task<PublishResult> PublishAsync(
            [NotNull] Exchange exchange,
            [NotNull] PublishInfo publishInfo,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create the given queue
        /// </summary>
        /// <param name="queueInfo"></param>
        /// <param name="vhost"></param>
        /// <param name="cancellationToken"></param>
        Task<Queue> CreateQueueAsync(
            [NotNull] QueueInfo queueInfo,
            [NotNull] Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete the given queue
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="cancellationToken"></param>
        Task DeleteQueueAsync(
            [NotNull] Queue queue,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all bindings on a given queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="cancellationToken"></param>
        Task<IEnumerable<Binding>> GetBindingsForQueueAsync(
            [NotNull] Queue queue,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Purge a queue of all messages
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="cancellationToken"></param>
        Task PurgeAsync(
            [NotNull] Queue queue,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get messages from a queue.
        /// 
        /// Please note that the publish / get paths in the HTTP API are intended for 
        /// injecting test messages, diagnostics etc - they do not implement reliable 
        /// delivery and so should be treated as a sysadmin's tool rather than a 
        /// general API for messaging.
        /// </summary>
        /// <param name="queue">The queue to retrieve from</param>
        /// <param name="criteria">The criteria for the retrieve</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Messages</returns>
        Task<IEnumerable<Message>> GetMessagesFromQueueAsync(
            [NotNull] Queue queue,
            GetMessagesCriteria criteria,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create a binding between an exchange and a queue
        /// </summary>
        /// <param name="exchange">the exchange</param>
        /// <param name="queue">the queue</param>
        /// <param name="bindingInfo">properties of the binding</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The binding that was created</returns>
        Task CreateBinding(
            [NotNull] Exchange exchange,
            [NotNull] Queue queue,
            [NotNull] BindingInfo bindingInfo,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create a binding between an exchange and an exchange
        /// </summary>
        /// <param name="sourceExchange">the source exchange</param>
        /// <param name="destinationExchange">the destination exchange</param>
        /// <param name="bindingInfo">properties of the binding</param>
        /// <param name="cancellationToken"></param>
        Task CreateBinding(
            [NotNull] Exchange sourceExchange,
            [NotNull] Exchange destinationExchange,
            [NotNull] BindingInfo bindingInfo,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all bindings between an exchange and a queue. 
        /// Remember, an exchange and a queue can be bound together many times!
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Binding>> GetBindingsAsync(
            [NotNull] Exchange exchange,
            [NotNull] Queue queue,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// A list of all bindings between an exchange and an exchange. 
        /// </summary>
        /// <param name="fromExchange"></param>
        /// <param name="toExchange"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Binding>> GetBindingsAsync(
            [NotNull] Exchange fromExchange,
            [NotNull] Exchange toExchange,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete the given binding
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="cancellationToken"></param>
        Task DeleteBindingAsync(
            [NotNull] Binding binding,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create a new virtual host
        /// </summary>
        /// <param name="virtualHostName">The name of the new virtual host</param>
        /// <param name="cancellationToken"></param>
        Task<Vhost> CreateVirtualHostAsync(
            string virtualHostName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete a virtual host
        /// </summary>
        /// <param name="vhost">The virtual host to delete</param>
        /// <param name="cancellationToken"></param>
        Task DeleteVirtualHostAsync(
            [NotNull] Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Enable tracing on given virtual host.
        /// </summary>
        /// <param name="vhost">The virtual host on which to enable tracing</param>
        Task EnableTracingAsync(
            [NotNull] Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Disables tracing on given virtual host.
        /// </summary>
        /// <param name="vhost">The virtual host on which to disable tracing</param>
        Task DisableTracingAsync(
            [NotNull] Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="userInfo">The user to create</param>
        /// <param name="cancellationToken"></param>
        Task<User> CreateUserAsync(
            [NotNull] UserInfo userInfo,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">The user to delete</param>
        /// <param name="cancellationToken"></param>
        Task DeleteUserAsync(
            [NotNull] User user,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create a permission
        /// </summary>
        /// <param name="permissionInfo">The permission to create</param>
        /// <param name="cancellationToken"></param>
        Task CreatePermissionAsync(
            [NotNull] PermissionInfo permissionInfo,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">The permission to delete</param>
        /// <param name="cancellationToken"></param>
        Task DeletePermissionAsync(
            [NotNull] Permission permission,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create a topic permission
        /// </summary>
        /// <param name="topicPermissionInfo">The topic permission to create</param>
        /// <param name="cancellationToken"></param>
        Task CreateTopicPermissionAsync(
            [NotNull] TopicPermissionInfo topicPermissionInfo,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete a topic permission
        /// </summary>
        /// <param name="permission">The topic permission to delete</param>
        /// <param name="cancellationToken"></param>
        Task DeleteTopicPermissionAsync(
            [NotNull] TopicPermission topicPermission,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Update the password of an user.
        /// </summary>
        /// <param name="userName">The name of a user</param>
        /// <param name="newPassword">The new password to set</param>
        /// <param name="cancellationToken"></param>
        Task<User> ChangeUserPasswordAsync(
            string userName,
            string newPassword,
            CancellationToken cancellationToken = default(CancellationToken));

        
        /// <summary>
        /// Declares a test queue, then publishes and consumes a message. Intended for use 
        /// by monitoring tools. If everything is working correctly, will return true.
        /// Note: the test queue will not be deleted (to to prevent queue churn if this 
        /// is repeatedly pinged).
        /// </summary>
        /// <param name="vhost"></param>
        /// <param name="cancellationToken"></param>
        Task<bool> IsAliveAsync(
            [NotNull] Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get an individual exchange by name
        /// </summary>
        /// <param name="exchangeName">The name of the exchange</param>
        /// <param name="vhost">The virtual host that contains the exchange</param>
        /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The exchange</returns>
        Task<Exchange> GetExchangeAsync(
            string exchangeName,
            [NotNull] Vhost vhost,
            GetRatesCriteria ratesCriteria = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get an individual queue by name
        /// </summary>
        /// <param name="queueName">The name of the queue</param>
        /// <param name="vhost">The virtual host that contains the queue</param>
        /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
        /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The Queue</returns>
        Task<Queue> GetQueueAsync(
            string queueName,
            [NotNull] Vhost vhost,
            GetLengthsCriteria lengthsCriteria = null,
            GetRatesCriteria ratesCriteria = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get an individual vhost by name
        /// </summary>
        /// <param name="vhostName">The VHost</param>
        /// <param name="cancellationToken"></param>
        Task<Vhost> GetVhostAsync(
            string vhostName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get a user by name
        /// </summary>
        /// <param name="userName">The name of the user</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The User</returns>
        Task<User> GetUserAsync(
            string userName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get collection of Policies on the cluster
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Policies</returns>
        Task<IEnumerable<Policy>> GetPoliciesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a policy on the cluster
        /// </summary>
        /// <param name="policy">Policy to create</param>
        /// <param name="cancellationToken"></param>
        Task CreatePolicy(
            [NotNull] Policy policy,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete a policy from the cluster
        /// </summary>
        /// <param name="policyName">Policy name</param>
        /// <param name="vhost">vhost on which the policy resides</param>
        /// <param name="cancellationToken"></param>
        Task DeletePolicyAsync(
            string policyName,
            Vhost vhost,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get all parameters on the cluster
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IEnumerable<Parameter>> GetParametersAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a parameter on the cluster
        /// </summary>
        /// <param name="parameter">Parameter to create</param>
        /// <param name="cancellationToken"></param>
        Task CreateParameterAsync(
            [NotNull]Parameter parameter,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete a parameter from the cluster
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="vhost"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        Task DeleteParameterAsync(
            string componentName,
            string vhost,
            string name,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get list of federations
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<Federation>> GetFederationAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}