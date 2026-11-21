using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.IntegrationTests;

[Collection("RabbitMQ")]
public class RecentApiTests(RabbitMqFixture fixture)
{
    private const string TestVhost = "recent_api_vhost";

    private IManagementClient Client => fixture.ManagementClient;

    private async Task<string> CreateVhostAsync(string name = TestVhost)
    {
        await Client.CreateVhostAsync(name);
        await Client.CreatePermissionAsync(name, fixture.User, new PermissionInfo());
        return name;
    }

    [Fact]
    public async Task Should_get_and_set_the_cluster_name()
    {
        var original = await Client.GetClusterNameAsync();

        await Client.SetClusterNameAsync(new ClusterName("recent-api-cluster"));
        (await Client.GetClusterNameAsync()).Name.Should().Be("recent-api-cluster");

        await Client.SetClusterNameAsync(original);
    }

    [Fact]
    public async Task Should_get_a_node_and_its_memory()
    {
        var name = (await Client.GetNodesAsync()).First().Name;

        (await Client.GetNodeAsync(name)).Name.Should().Be(name);
        (await Client.GetNodeMemoryAsync(name)).Memory.Should().ContainKey("total");
    }

    [Fact]
    public async Task Should_get_feature_flags()
    {
        var flags = await Client.GetFeatureFlagsAsync();

        flags.Should().Contain(f => f.Name == "quorum_queue" && f.State == "enabled");
    }

    [Fact]
    public async Task Should_get_deprecated_features()
    {
        if (fixture.RabbitmqVersion < new Version("3.13")) return;

        (await Client.GetDeprecatedFeaturesAsync()).Should().NotBeEmpty();
        (await Client.GetUsedDeprecatedFeaturesAsync()).Should().NotBeNull();
    }

    [Fact]
    public async Task Should_export_and_import_vhost_definitions()
    {
        var vhost = await CreateVhostAsync();
        await Client.CreateExchangeAsync(vhost, "exported-exchange", new ExchangeInfo("topic"));
        await Client.CreateQueueAsync(vhost, "exported", new QueueInfo(Arguments: new Dictionary<string, object?> { ["x-queue-type"] = "quorum" }));
        await Client.CreateQueueBindingAsync(vhost, "exported-exchange", "exported", new BindingInfo("orders.#"));
        await Client.CreatePolicyAsync(vhost, "ttl", new PolicyInfo("^exported$", new PolicyDefinition(MessageTtl: 1000), ApplyMode.Queues, 0));
        var definitions = await Client.GetDefinitionsAsync(vhost);
        await Client.DeleteVhostAsync(vhost);
        await CreateVhostAsync();

        await Client.ImportDefinitionsAsync(vhost, definitions);

        definitions.Queues.Should().ContainSingle(q => q.Name == "exported");
        (await Client.GetQueueAsync(vhost, "exported")).Type.Should().Be(Model.QueueType.Quorum);
        (await Client.GetQueueBindingsAsync(vhost, "exported-exchange", "exported")).Should().ContainSingle(b => b.RoutingKey == "orders.#");
        (await Client.GetPolicyAsync(vhost, "ttl")).Definition.MessageTtl.Should().Be(1000);
        await Client.DeleteVhostAsync(vhost);
    }

    [Fact]
    public async Task Should_get_detailed_queues()
    {
        if (fixture.RabbitmqVersion < new Version("4.0")) return;
        var vhost = await CreateVhostAsync();
        await Client.CreateQueueAsync(vhost, "detailed", new QueueInfo());

        var queues = await Client.GetDetailedQueuesAsync();

        queues.Should().ContainSingle(q => q.Name == "detailed" && q.Vhost == vhost);
        await Client.DeleteVhostAsync(vhost);
    }

    [Fact]
    public async Task Should_list_channels_and_connections_of_a_vhost_and_user()
    {
        var vhost = await CreateVhostAsync();

        (await Client.GetVhostChannelsAsync(vhost)).Should().BeEmpty();
        (await Client.GetUserConnectionsAsync("nobody")).Should().BeEmpty();
        await Client.CloseUserConnectionsAsync("nobody");
        await Client.DeleteVhostAsync(vhost);
    }

    [Fact]
    public async Task Should_get_permissions_by_vhost_and_user()
    {
        var vhost = await CreateVhostAsync();
        await Client.CreateTopicPermissionAsync(vhost, fixture.User, new TopicPermissionInfo(Exchange: "amq.topic"));

        (await Client.GetVhostPermissionsAsync(vhost)).Should().ContainSingle(p => p.User == fixture.User);
        (await Client.GetUserPermissionsAsync(fixture.User)).Should().Contain(p => p.Vhost == vhost);
        (await Client.GetPermissionAsync(vhost, fixture.User)).Configure.Should().Be(".*");
        (await Client.GetVhostTopicPermissionsAsync(vhost)).Should().ContainSingle(p => p.Exchange == "amq.topic");
        (await Client.GetUserTopicPermissionsAsync(fixture.User)).Should().Contain(p => p.Vhost == vhost);
        (await Client.GetTopicPermissionsAsync(vhost, fixture.User)).Should().ContainSingle(p => p.Exchange == "amq.topic");
        await Client.DeleteVhostAsync(vhost);
    }

    [Fact]
    public async Task Should_protect_a_vhost_from_deletion()
    {
        if (fixture.RabbitmqVersion < new Version("4.1")) return;
        var vhost = await CreateVhostAsync();

        await Client.EnableVhostDeletionProtectionAsync(vhost);
        var delete = () => Client.DeleteVhostAsync(vhost);
        await delete.Should().ThrowAsync<UnexpectedHttpStatusCodeException>();

        await Client.DisableVhostDeletionProtectionAsync(vhost);
        await Client.DeleteVhostAsync(vhost);
    }

    [Fact]
    public async Task Should_set_and_delete_vhost_limits()
    {
        var vhost = await CreateVhostAsync();

        await Client.CreateVhostMaxConnectionsLimitAsync(vhost, 10);
        await Client.CreateVhostMaxQueuesLimitAsync(vhost, 5);
        (await Client.GetVhostLimitsAsync(vhost)).Should().ContainSingle()
            .Which.Limits.Should().Be(new VhostLimitValues(MaxConnections: 10, MaxQueues: 5));
        (await Client.GetVhostLimitsAsync()).Should().Contain(l => l.Vhost == vhost);

        await Client.DeleteVhostMaxConnectionsLimitAsync(vhost);
        await Client.DeleteVhostMaxQueuesLimitAsync(vhost);
        (await Client.GetVhostLimitsAsync(vhost)).Should().BeEmpty();
        await Client.DeleteVhostAsync(vhost);
    }

    [Fact]
    public async Task Should_get_the_current_user()
    {
        var user = await Client.GetCurrentUserAsync();

        user.Name.Should().Be(fixture.User);
        user.Tags.Should().Contain(UserTags.Administrator);
    }

    [Fact]
    public async Task Should_list_and_bulk_delete_users_without_permissions()
    {
        await Client.CreateUserAsync("recent_a", UserInfo.ByPassword("a"));
        await Client.CreateUserAsync("recent_b", UserInfo.ByPassword("b"));

        (await Client.GetUsersWithoutPermissionsAsync()).Select(u => u.Name).Should().Contain(["recent_a", "recent_b"]);

        await Client.DeleteUsersAsync(["recent_a", "recent_b"]);
        (await Client.GetUsersAsync()).Should().NotContain(u => u.Name.StartsWith("recent_"));
    }

    [Fact]
    public async Task Should_get_the_policies_of_a_vhost()
    {
        var vhost = await CreateVhostAsync();
        await Client.CreatePolicyAsync(vhost, "ttl", new PolicyInfo("^ttl", new PolicyDefinition(MessageTtl: 1000), ApplyMode.Queues, 0));

        (await Client.GetPoliciesAsync(vhost)).Should().ContainSingle(p => p.Name == "ttl");
        (await Client.GetPolicyAsync(vhost, "ttl")).Definition.MessageTtl.Should().Be(1000);
        await Client.DeleteVhostAsync(vhost);
    }

    [Fact]
    public async Task Should_create_get_and_delete_operator_policies()
    {
        var vhost = await CreateVhostAsync();
        var policy = new Policy("max", "^", new PolicyDefinition(MaxLength: 100), vhost, ApplyMode.Queues);

        await Client.CreateOperatorPolicyAsync(policy);

        (await Client.GetOperatorPolicyAsync(vhost, "max")).Definition.MaxLength.Should().Be(100);
        (await Client.GetOperatorPoliciesAsync(vhost)).Should().ContainSingle(p => p.Name == "max");
        (await Client.GetOperatorPoliciesAsync()).Should().Contain(p => p.Vhost == vhost);
        await Client.DeleteOperatorPolicyAsync(vhost, "max");
        (await Client.GetOperatorPoliciesAsync(vhost)).Should().BeEmpty();
        await Client.DeleteVhostAsync(vhost);
    }

    [Fact]
    public async Task Should_create_get_and_delete_global_parameters()
    {
        await Client.CreateGlobalParameterAsync("recent_api", "a value");

        (await Client.GetGlobalParameterAsync("recent_api")).Value.ToString().Should().Be("a value");
        (await Client.GetGlobalParametersAsync()).Should().Contain(p => p.Name == "recent_api");
        await Client.DeleteGlobalParameterAsync("recent_api");
    }

    [Fact]
    public async Task Should_get_federations_of_a_vhost()
    {
        (await Client.GetFederationsAsync("/")).Should().BeEmpty();
    }

    [Fact]
    public async Task Should_run_the_listener_and_vhost_health_checks()
    {
        (await Client.IsListeningOnPortAsync(5672)).Should().BeTrue();
        (await Client.IsListeningOnPortAsync(5999)).Should().BeFalse();
        (await Client.IsListeningOnProtocolAsync("amqp")).Should().BeTrue();
        (await Client.AreAllVhostsRunningAsync()).Should().BeTrue();
        (await Client.HaveAnyCertificatesExpiringAsync(1, CertificateExpirationUnit.Months)).Should().BeFalse();
    }

    [Fact]
    public async Task Should_run_the_service_health_checks()
    {
        if (fixture.RabbitmqVersion < new Version("4.1")) return;

        (await Client.IsInServiceAsync()).Should().BeTrue();
        (await Client.IsBelowNodeConnectionLimitAsync()).Should().BeTrue();
        (await Client.IsReadyToServeClientsAsync()).Should().BeTrue();
    }
}
