using System;
using System.Collections.Generic;
using System.Threading;
using EasyNetQ.Management.Client.Model;
using JetBrains.Annotations;

namespace EasyNetQ.Management.Client;

public static class ManagementClientExtensions
{
    /// <summary>
    ///     Various random bits of information that describe the whole system.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Overview GetOverview(
        [NotNull] this IManagementClient source,
        GetLengthsCriteria lengthsCriteria = null,
        GetRatesCriteria ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetOverviewAsync(lengthsCriteria, ratesCriteria, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }


    /// <summary>
    ///     A list of nodes in the RabbitMQ cluster.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Node> GetNodes(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetNodesAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     The server definitions - exchanges, queues, bindings, users, virtual hosts, permissions.
    ///     Everything apart from messages.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Definitions GetDefinitions(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetDefinitionsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all open connections.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Connection> GetConnections(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetConnectionsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all open channels.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Channel> GetChannels(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetChannelsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Gets the channel. This returns more detail, including consumers than the GetChannels method.
    /// </summary>
    /// <param name="source"></param>
    /// <returns>The channel.</returns>
    /// <param name="channelName">Channel name.</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    public static Channel GetChannel(
        [NotNull] this IManagementClient source,
        string channelName,
        GetRatesCriteria ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetChannelAsync(channelName, ratesCriteria, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all exchanges.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Exchange> GetExchanges(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetExchangesAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all queues.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Queue> GetQueues(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetQueuesAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all queues for a virtual host.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Queue> GetQueues(
        [NotNull] this IManagementClient source, Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetQueuesAsync(vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all bindings.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Binding> GetBindings(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetBindingsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all vhosts.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Vhost> GetVhosts(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetVhostsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all users.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<User> GetUsers(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetUsersAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all permissions for all users.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Permission> GetPermissions(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetPermissionsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all topic permissions for all users.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<TopicPermission> GetTopicPermissions(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetTopicPermissionsAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Closes the given connection
    /// </summary>
    /// <param name="source"></param>
    /// <param name="connection"></param>
    /// <param name="cancellationToken"></param>
    public static void CloseConnection(
        [NotNull] this IManagementClient source,
        [NotNull] Connection connection,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.CloseConnectionAsync(connection, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Creates the given exchange
    /// </summary>
    /// <param name="source"></param>
    /// <param name="exchangeInfo"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    public static Exchange CreateExchange(
        [NotNull] this IManagementClient source,
        [NotNull] ExchangeInfo exchangeInfo,
        [NotNull] Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.CreateExchangeAsync(exchangeInfo, vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete the given exchange
    /// </summary>
    /// <param name="source"></param>
    /// <param name="exchange"></param>
    /// <param name="cancellationToken"></param>
    public static void DeleteExchange(
        [NotNull] this IManagementClient source,
        [NotNull] Exchange exchange,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.DeleteExchangeAsync(exchange, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all bindings in which a given exchange is the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="exchange"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Binding> GetBindingsWithSource(
        [NotNull] this IManagementClient source,
        [NotNull] Exchange exchange,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetBindingsWithSourceAsync(exchange, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all bindings in which a given exchange is the destination.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="exchange"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Binding> GetBindingsWithDestination(
        [NotNull] this IManagementClient source,
        [NotNull] Exchange exchange,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetBindingsWithDestinationAsync(exchange, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Publish a message to a given exchange.
    ///     Please note that the publish / get paths in the HTTP API are intended for injecting
    ///     test messages, diagnostics etc - they do not implement reliable delivery and so should
    ///     be treated as a sysadmin's tool rather than a general API for messaging.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="exchange">The exchange</param>
    /// <param name="publishInfo">The publication parameters</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A PublishResult, routed == true if the message was sent to at least one queue</returns>
    public static PublishResult Publish(
        [NotNull] this IManagementClient source,
        [NotNull] Exchange exchange,
        [NotNull] PublishInfo publishInfo,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.PublishAsync(exchange, publishInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create the given queue
    /// </summary>
    /// <param name="source"></param>
    /// <param name="queueInfo"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    public static Queue CreateQueue(
        [NotNull] this IManagementClient source,
        [NotNull] QueueInfo queueInfo,
        [NotNull] Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.CreateQueueAsync(queueInfo, vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete the given queue
    /// </summary>
    /// <param name="source"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    public static void DeleteQueue(
        [NotNull] this IManagementClient source,
        [NotNull] Queue queue,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.DeleteQueueAsync(queue, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all bindings on a given queue.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    public static IReadOnlyList<Binding> GetBindingsForQueue(
        [NotNull] this IManagementClient source,
        [NotNull] Queue queue,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetBindingsForQueueAsync(queue, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Purge a queue of all messages
    /// </summary>
    /// <param name="source"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    public static void Purge(
        [NotNull] this IManagementClient source,
        [NotNull] Queue queue,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.PurgeAsync(queue, cancellationToken)
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
    /// <param name="source"></param>
    /// <param name="queue">The queue to retrieve from</param>
    /// <param name="criteria">The criteria for the retrieve</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Messages</returns>
    public static IReadOnlyList<Message> GetMessagesFromQueue(
        [NotNull] this IManagementClient source,
        [NotNull] Queue queue,
        GetMessagesCriteria criteria,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetMessagesFromQueueAsync(queue, criteria, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create a binding between an exchange and a queue
    /// </summary>
    /// <param name="source"></param>
    /// <param name="exchange">the exchange</param>
    /// <param name="queue">the queue</param>
    /// <param name="bindingInfo">properties of the binding</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The binding that was created</returns>
    public static void CreateBinding(
        [NotNull] this IManagementClient source,
        [NotNull] Exchange exchange,
        [NotNull] Queue queue,
        [NotNull] BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.CreateBindingAsync(exchange, queue, bindingInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create a binding between an exchange and an exchange
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sourceExchange">the source exchange</param>
    /// <param name="destinationExchange">the destination exchange</param>
    /// <param name="bindingInfo">properties of the binding</param>
    /// <param name="cancellationToken"></param>
    public static void CreateBinding(
        [NotNull] this IManagementClient source,
        [NotNull] Exchange sourceExchange,
        [NotNull] Exchange destinationExchange,
        [NotNull] BindingInfo bindingInfo,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.CreateBindingAsync(sourceExchange, destinationExchange, bindingInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all bindings between an exchange and a queue.
    ///     Remember, an exchange and a queue can be bound together many times!
    /// </summary>
    /// <param name="source"></param>
    /// <param name="exchange"></param>
    /// <param name="queue"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Binding> GetBindings(
        [NotNull] this IManagementClient source,
        [NotNull] Exchange exchange,
        [NotNull] Queue queue,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetBindingsAsync(exchange, queue, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     A list of all bindings between an exchange and an exchange.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="fromExchange"></param>
    /// <param name="toExchange"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IReadOnlyList<Binding> GetBindings(
        [NotNull] this IManagementClient source,
        [NotNull] Exchange fromExchange,
        [NotNull] Exchange toExchange,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetBindingsAsync(fromExchange, toExchange, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete the given binding
    /// </summary>
    /// <param name="source"></param>
    /// <param name="binding"></param>
    /// <param name="cancellationToken"></param>
    public static void DeleteBinding(
        [NotNull] this IManagementClient source,
        [NotNull] Binding binding,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.DeleteBindingAsync(binding, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create a new virtual host
    /// </summary>
    /// <param name="source"></param>
    /// <param name="vhostName">The name of the new virtual host</param>
    /// <param name="cancellationToken"></param>
    public static Vhost CreateVhost(
        [NotNull] this IManagementClient source,
        string vhostName,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.CreateVhostAsync(vhostName, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a virtual host
    /// </summary>
    /// <param name="source"></param>
    /// <param name="vhost">The virtual host to delete</param>
    /// <param name="cancellationToken"></param>
    public static void DeleteVhost(
        [NotNull] this IManagementClient source,
        [NotNull] Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.DeleteVhostAsync(vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Enable tracing on given virtual host.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="vhost">The virtual host on which to enable tracing</param>
    /// <param name="cancellationToken"></param>
    public static void EnableTracing(
        [NotNull] this IManagementClient source,
        [NotNull] Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.EnableTracingAsync(vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Disables tracing on given virtual host.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="vhost">The virtual host on which to disable tracing</param>
    /// <param name="cancellationToken"></param>
    public static void DisableTracing(
        [NotNull] this IManagementClient source,
        [NotNull] Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.DisableTracingAsync(vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create a new user
    /// </summary>
    /// <param name="source"></param>
    /// <param name="userInfo">The user to create</param>
    /// <param name="cancellationToken"></param>
    public static User CreateUser(
        [NotNull] this IManagementClient source,
        [NotNull] UserInfo userInfo,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.CreateUserAsync(userInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a user
    /// </summary>
    /// <param name="source"></param>
    /// <param name="user">The user to delete</param>
    /// <param name="cancellationToken"></param>
    public static void DeleteUser(
        [NotNull] this IManagementClient source,
        [NotNull] User user,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.DeleteUserAsync(user, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create a permission
    /// </summary>
    /// <param name="source"></param>
    /// <param name="permissionInfo">The permission to create</param>
    /// <param name="cancellationToken"></param>
    public static void CreatePermission(
        [NotNull] this IManagementClient source,
        [NotNull] PermissionInfo permissionInfo,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.CreatePermissionAsync(permissionInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a permission
    /// </summary>
    /// <param name="source"></param>
    /// <param name="permission">The permission to delete</param>
    /// <param name="cancellationToken"></param>
    public static void DeletePermission(
        [NotNull] this IManagementClient source,
        [NotNull] Permission permission,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.DeletePermissionAsync(permission, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Create a topic permission
    /// </summary>
    /// <param name="source"></param>
    /// <param name="topicPermissionInfo">The topic permission to create</param>
    /// <param name="cancellationToken"></param>
    public static void CreateTopicPermission(
        [NotNull] this IManagementClient source,
        [NotNull] TopicPermissionInfo topicPermissionInfo,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.CreateTopicPermissionAsync(topicPermissionInfo, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a topic permission
    /// </summary>
    /// <param name="source"></param>
    /// <param name="permission">The topic permission to delete</param>
    /// <param name="cancellationToken"></param>
    public static void DeleteTopicPermission(
        [NotNull] this IManagementClient source,
        [NotNull] TopicPermission permission,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.DeleteTopicPermissionAsync(permission, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Update the password of an user.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="userName">The name of a user</param>
    /// <param name="newPassword">The new password to set</param>
    /// <param name="cancellationToken"></param>
    public static User ChangeUserPassword(
        [NotNull] this IManagementClient source,
        string userName,
        string newPassword,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.ChangeUserPasswordAsync(userName, newPassword, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }


    /// <summary>
    ///     Declares a test queue, then publishes and consumes a message. Intended for use
    ///     by monitoring tools. If everything is working correctly, will return true.
    ///     Note: the test queue will not be deleted (to to prevent queue churn if this
    ///     is repeatedly pinged).
    /// </summary>
    /// <param name="source"></param>
    /// <param name="vhost"></param>
    /// <param name="cancellationToken"></param>
    public static bool IsAlive(
        [NotNull] this IManagementClient source,
        [NotNull] Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.IsAliveAsync(vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get an individual exchange by name
    /// </summary>
    /// <param name="source"></param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="vhost">The virtual host that contains the exchange</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The exchange</returns>
    public static Exchange GetExchange(
        [NotNull] this IManagementClient source,
        string exchangeName,
        [NotNull] Vhost vhost,
        GetRatesCriteria ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetExchangeAsync(exchangeName, vhost, ratesCriteria, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get an individual queue by name
    /// </summary>
    /// <param name="source"></param>
    /// <param name="queueName">The name of the queue</param>
    /// <param name="vhost">The virtual host that contains the queue</param>
    /// <param name="lengthsCriteria">Criteria for getting samples of queue length data</param>
    /// <param name="ratesCriteria">Criteria for getting samples of rate data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The Queue</returns>
    public static Queue GetQueue(
        [NotNull] this IManagementClient source,
        string queueName,
        [NotNull] Vhost vhost,
        GetLengthsCriteria lengthsCriteria = null,
        GetRatesCriteria ratesCriteria = null,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetQueueAsync(queueName, vhost, lengthsCriteria, ratesCriteria, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get an individual vhost by name
    /// </summary>
    /// <param name="source"></param>
    /// <param name="vhostName">The VHost</param>
    /// <param name="cancellationToken"></param>
    public static Vhost GetVhost(
        [NotNull] this IManagementClient source,
        string vhostName,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetVhostAsync(vhostName, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get a user by name
    /// </summary>
    /// <param name="source"></param>
    /// <param name="userName">The name of the user</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The User</returns>
    public static User GetUser(
        [NotNull] this IManagementClient source,
        string userName,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetUserAsync(userName, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get collection of Policies on the cluster
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Policies</returns>
    public static IReadOnlyList<Policy> GetPolicies(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetPoliciesAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Creates a policy on the cluster
    /// </summary>
    /// <param name="source"></param>
    /// <param name="policy">Policy to create</param>
    /// <param name="cancellationToken"></param>
    public static void CreatePolicy(
        [NotNull] this IManagementClient source,
        [NotNull] Policy policy,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.CreatePolicyAsync(policy, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a policy from the cluster
    /// </summary>
    /// <param name="source"></param>
    /// <param name="policyName">Policy name</param>
    /// <param name="vhost">vhost on which the policy resides</param>
    /// <param name="cancellationToken"></param>
    public static void DeletePolicy(
        [NotNull] this IManagementClient source,
        string policyName,
        Vhost vhost,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.DeletePolicyAsync(policyName, vhost, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get all parameters on the cluster
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    public static IReadOnlyList<Parameter> GetParameters(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetParametersAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Creates a parameter on the cluster
    /// </summary>
    /// <param name="source"></param>
    /// <param name="parameter">Parameter to create</param>
    /// <param name="cancellationToken"></param>
    public static void CreateParameter(
        [NotNull] this IManagementClient source,
        [NotNull] Parameter parameter,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.CreateParameterAsync(parameter, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Delete a parameter from the cluster
    /// </summary>
    /// <param name="source"></param>
    /// <param name="componentName"></param>
    /// <param name="vhostName"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    public static void DeleteParameter(
        [NotNull] this IManagementClient source,
        string componentName,
        string vhostName,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        source.DeleteParameterAsync(componentName, vhostName, name, cancellationToken)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Get list of federations
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static List<Federation> GetFederation(
        [NotNull] this IManagementClient source,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.GetFederationAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();
    }
}
