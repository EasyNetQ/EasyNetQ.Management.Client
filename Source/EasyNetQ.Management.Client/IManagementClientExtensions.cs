using System.Net;
using EasyNetQ.Management.Client.Model;
using Refit;

namespace EasyNetQ.Management.Client;

public static partial class IManagementClientExtensions
{
    // Explicitly define extensions for methods with identical parameters.
    // Code generator will produce unwanted extensions, like DeletePermissionAsync(TopicPermission) and DeleteTopicPermissionAsync(Permission).
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
    ///     Delete a topic permission
    /// </summary>
    /// <param name="client"></param>
    /// <param name="topicPermission"></param>
    /// <param name="cancellationToken"></param>
    public static Task DeleteTopicPermissionAsync(
        this IManagementClient client,
        TopicPermission topicPermission,
        CancellationToken cancellationToken = default
    ) => client.DeleteTopicPermissionAsync(topicPermission.Vhost, topicPermission.User, cancellationToken);


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
        policy.Name,
        new PolicyInfo(
            ApplyTo: policy.ApplyTo,
            Definition: policy.Definition,
            Priority: policy.Priority,
            Pattern: policy.Pattern
        ),
        cancellationToken
    );

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

        var userInfo = new UserInfo(newPassword, null, user.Tags);

        await client.CreateUserAsync(userName, userInfo, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Creates a shovel in a specific vhost
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="shovelName"></param>
    /// <param name="shovelDescription"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task CreateShovelAsync(
        this IManagementClient client,
        string vhostName,
        string shovelName,
        ParameterShovelValue shovelDescription,
        CancellationToken cancellationToken = default
    ) => client.CreateParameterAsync("shovel", vhostName, shovelName, new ParameterValue(shovelDescription), cancellationToken);

    /// <summary>
    ///     Get specific shovel parameters from a specific vhost
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="shovelName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<Parameter> GetShovelAsync(
        this IManagementClient client,
        string vhostName,
        string shovelName,
        CancellationToken cancellationToken = default
    ) => client.GetParameterAsync(vhostName, "shovel", shovelName, cancellationToken);

    /// <summary>
    ///     Creates a federation upstream in a specific vhost
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="federationUpstreamName"></param>
    /// <param name="federationUpstreamDescription"></param>
    /// <param name="cancellationToken"></param>
    public static Task CreateFederationUpstreamAsync(
        this IManagementClient client,
        string vhostName,
        string federationUpstreamName,
        ParameterFederationValue federationUpstreamDescription,
        CancellationToken cancellationToken = default
    ) => client.CreateParameterAsync("federation-upstream", vhostName, federationUpstreamName, new ParameterValue(federationUpstreamDescription), cancellationToken);

    /// <summary>
    ///     Get a specific federation upstream in a specific vhost
    /// </summary>
    /// <param name="client"></param>
    /// <param name="vhostName"></param>
    /// <param name="federationUpstreamName"></param>
    /// <param name="cancellationToken"></param>
    public static Task<Parameter> GetFederationUpstreamAsync(
        this IManagementClient client,
        string vhostName,
        string federationUpstreamName,
        CancellationToken cancellationToken = default
    ) => client.GetParameterAsync(vhostName, "federation-upstream", federationUpstreamName, cancellationToken);

    /// <summary>
    ///     Creates a parameter on the cluster. Under Native AOT the value must be a type the client serializes, see <see cref="ParameterValue"/>.
    /// </summary>
    public static Task CreateParameterAsync(
        this IManagementClient client,
        string componentName,
        string vhostName,
        string parameterName,
        object value,
        CancellationToken cancellationToken = default
    ) => client.CreateParameterAsync(componentName, vhostName, parameterName, new ParameterValue(value), cancellationToken);

    /// <summary>
    ///     Enable tracing on given virtual host.
    /// </summary>
    public static Task EnableTracingAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.CreateVhostAsync(vhostName, new VhostInfo(Tracing: true), cancellationToken);

    /// <summary>
    ///     Disables tracing on given virtual host.
    /// </summary>
    public static Task DisableTracingAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => client.CreateVhostAsync(vhostName, new VhostInfo(Tracing: false), cancellationToken);

    /// <summary>
    ///     Sets per-user max connections limit for user.
    /// </summary>
    public static Task CreateUserMaxConnectionsLimitAsync(
        this IManagementClient client,
        string userName,
        int maxConnections,
        CancellationToken cancellationToken = default
    ) => client.CreateUserMaxConnectionsLimitAsync(userName, new LimitInfo(maxConnections), cancellationToken);

    /// <summary>
    ///     Sets per-user max channels limit for user.
    /// </summary>
    public static Task CreateUserMaxChannelsLimitAsync(
        this IManagementClient client,
        string userName,
        int maxChannels,
        CancellationToken cancellationToken = default
    ) => client.CreateUserMaxChannelsLimitAsync(userName, new LimitInfo(maxChannels), cancellationToken);

    /// <summary>
    ///     Declares a test queue, then publishes and consumes a message. Intended for use
    ///     by monitoring tools. If everything is working correctly, will return true.
    ///     Note: the test queue will not be deleted (to prevent queue churn if this
    ///     is repeatedly pinged).
    /// </summary>
    public static async Task<bool> IsAliveAsync(
        this IManagementClient client,
        string vhostName,
        CancellationToken cancellationToken = default
    ) => Passed(await client.CheckAlivenessAsync(vhostName, cancellationToken).ConfigureAwait(false));

    /// <summary>
    ///     Returns true if there are any alarms in effect in the cluster
    /// </summary>
    public static async Task<bool> HaveAnyClusterAlarmsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => !Passed(await client.CheckAlarmsAsync(cancellationToken).ConfigureAwait(false));

    /// <summary>
    ///     Returns true if there are any alarms in effect in on a target node
    /// </summary>
    public static async Task<bool> HaveAnyLocalAlarmsAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => !Passed(await client.CheckLocalAlarmsAsync(cancellationToken).ConfigureAwait(false));

    /// <summary>
    ///     Returns true if there are quorum queues with minimum online quorum (queues that
    ///     would lose their quorum and availability if the target node is shut down)
    /// </summary>
    public static async Task<bool> HaveAnyQuorumQueuesInCriticalStateAsync(
        this IManagementClient client,
        CancellationToken cancellationToken = default
    ) => !Passed(await client.CheckQuorumCriticalAsync(cancellationToken).ConfigureAwait(false));

    // A health check answers 200 when it passes and 503 when it fails; anything else is an error.
    private static bool Passed(IApiResponse response)
    {
        using (response)
            return response.StatusCode switch
            {
                HttpStatusCode.OK => true,
                HttpStatusCode.ServiceUnavailable => false,
                { } statusCode => throw UnexpectedHttpStatusCodeException.From(statusCode, response),
                null => throw (Exception?)response.Error ?? new HttpRequestException("No response was received")
            };
    }
}
