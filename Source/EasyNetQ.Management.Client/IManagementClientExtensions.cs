using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client;

public static class IManagementClientExtensions
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
        new PolicyInfo(
            Name: policy.Name,
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

        var userInfo = new UserInfo(userName, newPassword, null, user.Tags);

        await client.CreateUserAsync(userInfo, cancellationToken).ConfigureAwait(false);
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
    ) => client.CreateParameterAsync("shovel", vhostName, shovelName, shovelDescription, cancellationToken);

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
    ) => client.CreateParameterAsync("federation-upstream", vhostName, federationUpstreamName, federationUpstreamDescription, cancellationToken);

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
}
