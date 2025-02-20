using EasyNetQ.Management.Client.Model;
using FluentAssertions.Extensions;
using System.Security.Cryptography;
using System.Text.Json;

namespace EasyNetQ.Management.Client.IntegrationTests;

using JsonSerializer = System.Text.Json.JsonSerializer;

[Collection("RabbitMQ")]
public class ManagementClientTests
{
    private readonly RabbitMqFixture fixture;
    private readonly ITestOutputHelper output;
    private static readonly Vhost Vhost = new(Name: "/");

    public ManagementClientTests(RabbitMqFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.output = output;
    }

    private const string TestExchange = "management_api_test_exchange";
    private const string TestExchange2 = "management_api_test_exchange2";
    private const string TestExchangeTestQueueWithPlusChar = "management_api_test_exchange+plus+test";
    private const string TestQueue = "management_api_test_queue";
    private const string TestQueueWithPlusChar = "management_api_test_queue+plus+test";
    private const string TestUser = "mikey";

    private async Task<Exchange> CreateExchange(string exchangeName)
    {
        await fixture.ManagementClient.CreateExchangeAsync(Vhost, exchangeName, new ExchangeInfo("direct"));
        return await fixture.ManagementClient.GetExchangeAsync(Vhost, exchangeName);
    }

    private async Task<Queue> EnsureQueueExists(string managementApiTestQueue)
    {
        return (await fixture.ManagementClient.GetQueuesAsync())
               .SingleOrDefault(x => x.Name == managementApiTestQueue)
               ?? await CreateTestQueue(managementApiTestQueue);
    }

    private async Task<Queue> CreateTestQueue(string queueName)
    {
        var vhost = await fixture.ManagementClient.GetVhostAsync(Vhost.Name);
        return await CreateTestQueueInVhost(queueName, vhost);
    }

    private async Task<Queue> CreateTestQueueInVhost(string queueName, Vhost vhost)
    {
        var queueInfo = new QueueInfo();
        await fixture.ManagementClient.CreateQueueAsync(vhost, queueName, queueInfo);
        return await fixture.ManagementClient.GetQueueAsync(vhost, queueName);
    }

    private async Task<Exchange> EnsureExchangeExists(string exchangeName)
    {
        return (await fixture.ManagementClient.GetExchangesAsync())
            .SingleOrDefault(x => x.Name == exchangeName) ?? await CreateExchange(exchangeName);
    }

    private const string TestVHost = "management_test_virtual_host";

    [Fact]
    public async Task Should_be_able_to_change_the_password_of_a_user()
    {
        var userInfo = UserInfo.ByPassword("topSecret").AddTag(UserTags.Monitoring).AddTag(UserTags.Management);

        await fixture.ManagementClient.CreateUserAsync(TestUser, userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(TestUser);
        await fixture.ManagementClient.ChangeUserPasswordAsync(TestUser, "newPassword");
        var updatedUser = await fixture.ManagementClient.GetUserAsync(TestUser);

        updatedUser.Should().BeEquivalentTo(user, options => options.Excluding(o => o.PasswordHash));
        updatedUser.PasswordHash.Should().NotBe(user.PasswordHash);
    }

    [Fact(Skip = "Requires at least a connection")]
    public async Task Should_be_able_to_close_connection()
    {
        // first get a connection
        var connections = await fixture.ManagementClient.GetConnectionsAsync();

        // then close it
        await fixture.ManagementClient.CloseConnectionAsync(connections.First());
    }

    [Fact]
    public async Task Should_be_able_to_configure_request()
    {
        using var client = new ManagementClient(
            fixture.Endpoint, fixture.User, fixture.Password, configureHttpRequestMessage: req => req.Headers.Add("x-not-used", "some_value")
        );

        await client.GetOverviewAsync();
    }

    [Fact]
    public async Task Should_be_able_to_create_a_queue()
    {
        await fixture.ManagementClient.CreateQueueAsync(Vhost, TestQueue, new QueueInfo());
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, TestQueue);

        queue.Name.Should().Be(TestQueue);
    }

    [Fact]
    public async Task Should_be_able_to_create_a_queue_with_arguments()
    {
        const string exchangeName = "test-dead-letter-exchange";
        const string argumentKey = "x-dead-letter-exchange";
        var queueName = $"{TestQueue}1";
        var queueInfo = new QueueInfo(Arguments: new Dictionary<string, object?> { { argumentKey, exchangeName } });

        await fixture.ManagementClient.CreateQueueAsync(Vhost, queueName, queueInfo);
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, queueName);
        queue.Arguments[argumentKey].Should().NotBeNull();
        queue.Arguments[argumentKey].Should().Be(exchangeName);
    }

    [Fact]
    public async Task Should_be_able_to_create_a_queue_with_plus_char_in_the_name()
    {
        var queueInfo = new QueueInfo();

        await fixture.ManagementClient.CreateQueueAsync(Vhost, TestQueueWithPlusChar, queueInfo);
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, TestQueueWithPlusChar);

        queue.Name.Should().Be(TestQueueWithPlusChar);
    }

    [Fact]
    public async Task Should_be_able_to_check_cluster_alarms()
    {
        var result = await fixture.ManagementClient.HaveAnyClusterAlarmsAsync();
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Should_be_able_to_check_local_alarms()
    {
        var result = await fixture.ManagementClient.HaveAnyClusterAlarmsAsync();
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Should_be_able_to_check_classic_queues_without_synchronised_mirrors()
    {
        var result = await fixture.ManagementClient.HaveAnyClassicQueuesWithoutSynchronisedMirrorsAsync();
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Should_be_able_to_check_quorum_queues_is_critical_state()
    {
        var result = await fixture.ManagementClient.HaveAnyQuorumQueuesInCriticalStateAsync();
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Should_be_able_to_rebalance_queues()
    {
        await fixture.ManagementClient.RebalanceQueuesAsync();
    }

    [Fact]
    public async Task Should_be_able_to_create_a_user_with_password()
    {
        var userInfo = UserInfo.ByPassword("topSecret").AddTag(UserTags.Administrator);

        await fixture.ManagementClient.CreateUserAsync(TestUser, userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(TestUser);

        user.Name.Should().Be(TestUser);
        user.Should().BeEquivalentTo(userInfo with { HashingAlgorithm = HashAlgorithmName.SHA256 },
            options => options.ExcludingMissingMembers().Excluding(o => o.PasswordHash));
    }

    [Fact]
    public async Task Should_be_able_to_create_a_user_with_password_hash()
    {
        var testUser = "hash_user";
        var passwordHash = UserInfo.ComputePasswordHash("topSecret");
        var userInfo = UserInfo.ByPasswordHash(passwordHash).AddTag(UserTags.Administrator);

        await fixture.ManagementClient.CreateUserAsync(testUser, userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(testUser);

        user.Name.Should().Be(testUser);
        user.Should().BeEquivalentTo(userInfo with { HashingAlgorithm = HashAlgorithmName.SHA256 },
            options => options.ExcludingMissingMembers());

        // validate authentication
        var mc = new ManagementClient(fixture.ManagementClient.Endpoint, testUser, "topSecret");
        await mc.GetUserAsync(testUser);
    }

    [Fact]
    public async Task Should_be_able_to_create_a_user_with_password_and_hash_algorithm()
    {
        var testUser = "hash_user";
        var userInfo = UserInfo.ByPasswordAndHashAlgorithm("topSecret", HashAlgorithmName.SHA512).AddTag(UserTags.Administrator);

        await fixture.ManagementClient.CreateUserAsync(testUser, userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(testUser);

        user.Name.Should().Be(testUser);
        user.Should().BeEquivalentTo(userInfo, options => options.ExcludingMissingMembers());

        // validate authentication
        var mc = new ManagementClient(fixture.ManagementClient.Endpoint, testUser, "topSecret");
        await mc.GetUserAsync(testUser);
    }

    [Fact]
    public async Task Should_be_able_to_create_a_user_with_the_policymaker_tag()
    {
        var userInfo = UserInfo.ByPassword("topSecret").AddTag(UserTags.Policymaker);

        await fixture.ManagementClient.CreateUserAsync(TestUser, userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(TestUser);

        user.Name.Should().Be(TestUser);
        user.Should().BeEquivalentTo(userInfo with { HashingAlgorithm = HashAlgorithmName.SHA256 },
            options => options.ExcludingMissingMembers().Excluding(o => o.PasswordHash));
    }

    [Fact]
    public async Task Should_be_able_to_create_a_user_with_empty_password_hash()
    {
        var testUser = "empty";
        var userInfo = UserInfo.ByPasswordHash("").AddTag(UserTags.Administrator);

        await fixture.ManagementClient.CreateUserAsync(testUser, userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(testUser);

        user.Name.Should().Be(testUser);
        user.Should().BeEquivalentTo(userInfo with { HashingAlgorithm = HashAlgorithmName.SHA256 },
            options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Should_be_able_to_create_a_user_without_password_and_password_hash()
    {
        var testUser = "empty";
        var userInfo = new UserInfo(null, null, Array.Empty<string>()).AddTag(UserTags.Administrator);

        await fixture.ManagementClient.CreateUserAsync(testUser, userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(testUser);

        user.Name.Should().Be(testUser);
        user.Should().BeEquivalentTo(userInfo with { HashingAlgorithm = HashAlgorithmName.SHA256 },
            options => options.ExcludingMissingMembers().Excluding(o => o.PasswordHash));
    }

    [Fact]
    public async Task Should_throw_when_trying_to_create_a_user_with_password_and_password_hash()
    {
        var testUser = "empty";
        var userInfo = new UserInfo("a", "b", Array.Empty<string>()).AddTag(UserTags.Administrator);

        var act = async () => await fixture.ManagementClient.CreateUserAsync(testUser, userInfo);

        await act.Should().ThrowAsync<UnexpectedHttpStatusCodeException>()
            .Where(e => e.StatusCode == System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_be_able_to_create_all_the_definitions_in_a_policy()
    {
        const string policyName = "a-sample-all-definitions-in-a-policy";
        const int priority = 999;
        const HaMode haMode = HaMode.All;
        const HaSyncMode haSyncMode = HaSyncMode.Automatic;
        const HaPromote haPromoteOnFailure = HaPromote.Always;
        const HaPromote haPromoteOnShutdown = HaPromote.WhenSynced;
        QueueVersion queueVersion = fixture.RabbitmqVersion >= new Version("3.12") ? QueueVersion.V2 : QueueVersion.V1;
        const QueueLocator queueMasterLocator = QueueLocator.ClientLocal;
        const uint deliveryLimit = 3;
        const Model.DeadLetterStrategy deadLetterStrategy = Model.DeadLetterStrategy.AtLeastOnce;
        const QueueLocator queueLeaderLocator = QueueLocator.Balanced;
        const string maxAge = "1h";
        const uint streamMaxSegmentSizeBytes = 50000;
        const string alternateExchange = "a-sample-alternate-exchange";
        const string deadLetterExchange = "a-sample-dead-letter-exchange";
        const string deadLetterRoutingKey = "a-sample-dead-letter-exchange-key";
        const uint messageTtl = 5000;
        const uint expires = 10000;
        const uint maxLength = 500;
        const long maxLengthBytes = 5000;
        const Overflow overflow = Overflow.RejectPublish;
        uint? consumerTimeout = fixture.RabbitmqVersion >= new Version("3.12") ? 3600000 : null;
        Dictionary<string, object?> extensionData = new Dictionary<string, object?> { { "max-in-memory-length", 1000000 } };

        Policy policy =
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                Definition: new PolicyDefinition(
                    HaMode: haMode,
                    HaSyncMode: haSyncMode,
                    HaPromoteOnFailure: haPromoteOnFailure,
                    HaPromoteOnShutdown: haPromoteOnShutdown,
                    QueueVersion: queueVersion,
                    QueueMasterLocator: queueMasterLocator,

                    DeliveryLimit: deliveryLimit,
                    DeadLetterStrategy: deadLetterStrategy,
                    QueueLeaderLocator: queueLeaderLocator,

                    MaxAge: maxAge,
                    StreamMaxSegmentSizeBytes: streamMaxSegmentSizeBytes,

                    AlternateExchange: alternateExchange,

                    DeadLetterExchange: deadLetterExchange,
                    DeadLetterRoutingKey: deadLetterRoutingKey,
                    MessageTtl: messageTtl,
                    Expires: expires,
                    MaxLength: maxLength,
                    MaxLengthBytes: maxLengthBytes,
                    Overflow: overflow,
                    ConsumerTimeout: consumerTimeout
                )
                { ExtensionData = extensionData },
                Priority: priority
            );

        await fixture.ManagementClient.CreatePolicyAsync(policy);

        var policies = await fixture.ManagementClient.GetPoliciesAsync();
        policies.Should().ContainEquivalentOf(policy, options => options.Excluding(o => o.Definition.JsonExtensionData));
    }

    [Fact]
    public async Task Should_be_able_to_create_alternate_exchange_policy()
    {
        const string policyName = "a-sample-alternate-exchange-policy";
        const string alternateExchange = "a-sample-alternate-exchange";
        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                Definition: new PolicyDefinition(
                    AlternateExchange: alternateExchange
                )
            )
        );
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Definition.AlternateExchange == alternateExchange)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_an_exchange()
    {
        var exchange = await CreateExchange(TestExchange);
        exchange.Name.Should().Be(TestExchange);
    }

    [Fact]
    public async Task Should_be_able_to_create_an_exchange_with_plus_char_in_the_name()
    {
        var exchangeInfo = new ExchangeInfo("direct");

        await fixture.ManagementClient.CreateExchangeAsync(Vhost, TestExchangeTestQueueWithPlusChar, exchangeInfo);
        var exchange = await fixture.ManagementClient.GetExchangeAsync(Vhost, TestExchangeTestQueueWithPlusChar);

        exchange.Name.Should().Be(TestExchangeTestQueueWithPlusChar);
    }

    [Fact]
    public async Task Should_be_able_to_create_dead_letter_exchange_policy()
    {
        const string policyName = "a-sample-dead-letter-exchange";
        const string deadLetterExchange = "a-sample-dead-letter-exchange";
        const string deadLetterRoutingKey = "a-sample-dead-letter-exchange-key";
        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                Definition: new PolicyDefinition(
                    DeadLetterExchange: deadLetterExchange,
                    DeadLetterRoutingKey: deadLetterRoutingKey
                )
            )
        );
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Definition.DeadLetterExchange == deadLetterExchange
                 && p.Definition.DeadLetterRoutingKey == deadLetterRoutingKey)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_exchanges_only_policies()
    {
        const string policyName = "asamplepolicy-exchange-only";
        const HaMode haMode = HaMode.All;
        const HaSyncMode haSyncMode = HaSyncMode.Automatic;

        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                ApplyTo: ApplyMode.Exchanges,
                Definition: new PolicyDefinition(
                    HaMode: haMode,
                    HaSyncMode: haSyncMode
                )
            )
        );
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.ApplyTo == ApplyMode.Exchanges
                 && p.Definition.HaMode == haMode
                 && p.Definition.HaSyncMode == haSyncMode)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_expires_policy()
    {
        const string policyName = "a-sample-expires";
        const uint expires = 10000;
        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                Definition: new PolicyDefinition(
                    Expires: expires
                )
            )
        );
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Definition.Expires == expires)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_federation_upstream_policy()
    {
        const string policyName = "a-sample-federation-upstream-policy";

        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                Definition: new PolicyDefinition(
                    FederationUpstream: "my-upstream"
                )
            )
        );
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Definition.FederationUpstream == "my-upstream")
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_federation_upstream_set_policy()
    {
        const string policyName = "a-sample-federation-upstream-set-policy";

        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                Definition: new PolicyDefinition(
                    FederationUpstreamSet: "my-upstream-set"
                )
            )
        );
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Definition.FederationUpstreamSet == "my-upstream-set")
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_max_length_policy()
    {
        const string policyName = "a-sample-max-length";
        const uint maxLength = 500;
        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                Definition: new PolicyDefinition(
                    MaxLength: maxLength
                )
            )
        );
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Definition.MaxLength == maxLength)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_max_length_bytes_policy()
    {
        const string policyName = "a-sample-max-length-bytes";
        const uint maxLengthBytes = 500;
        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                Definition: new PolicyDefinition(
                    MaxLengthBytes: maxLengthBytes
                )
            )
        );
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Definition.MaxLengthBytes == maxLengthBytes)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_overflow_policy()
    {
        foreach (Overflow overflow in Enum.GetValues(typeof(Overflow)))
        {
            const string policyName = "a-sample-overflow";
            const uint maxLengthBytes = 500;
            await fixture.ManagementClient.CreatePolicyAsync(
                new Policy(
                    Name: policyName,
                    Pattern: "averyuncommonpattern",
                    Vhost: Vhost.Name,
                    Definition: new PolicyDefinition(
                        MaxLengthBytes: maxLengthBytes,
                        Overflow: overflow
                    )
                )
            );
            Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
                p => p.Name == policyName
                     && p.Vhost == Vhost.Name
                     && p.Definition.MaxLengthBytes == maxLengthBytes
                     && p.Definition.Overflow == overflow)
            );
        }
    }

    [Fact]
    public async Task Should_be_able_to_create_message_ttl_policy()
    {
        const string policyName = "a-sample-message-ttl";
        const uint messageTtl = 5000;
        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                Definition: new PolicyDefinition(
                    MessageTtl: messageTtl
                )
            )
        );
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Definition.MessageTtl == messageTtl)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_federation_upstream_parameter()
    {
        await fixture.ManagementClient.CreateParameterAsync(
            new Parameter
            (
                Component: "federation-upstream",
                Name: "myfakefederationupstream1",
                Vhost: Vhost.Name,
                Value: new { Uri = $"amqp://{fixture.User}:{fixture.Password}@{fixture.Endpoint.Host}" }
            )
        );
        Assert.Contains(await fixture.ManagementClient.GetParametersAsync(), p => p.Name == "myfakefederationupstream1");
    }

    [Fact]
    public async Task Should_be_able_to_create_federation_upstream_parameter_with_extension_method()
    {
        var amqpUri = new AmqpUri(fixture.Endpoint.Host, fixture.Endpoint.Port, fixture.User, fixture.Password);

        await fixture.ManagementClient.CreateFederationUpstreamAsync(
            vhostName: Vhost.Name,
            federationUpstreamName: "myfakefederationupstream-extension",
            federationUpstreamDescription: new ParameterFederationValue(amqpUri, Expires: 3600000)
        );

        var parameter = await fixture.ManagementClient.GetFederationUpstreamAsync(Vhost.Name, "myfakefederationupstream-extension");
        Assert.Equal("myfakefederationupstream-extension", parameter.Name);
    }

    [Fact]
    public async Task Should_be_able_to_create_permissions()
    {
        var user = (await fixture.ManagementClient.GetUsersAsync()).SingleOrDefault(x => x.Name == TestUser);
        if (user == null)
            throw new EasyNetQTestException($"user '{TestUser}' hasn't been created");

        var vhost = (await fixture.ManagementClient.GetVhostsAsync()).SingleOrDefault(x => x.Name == TestVHost);
        if (vhost == null)
            throw new EasyNetQTestException($"Test vhost: '{TestVHost}' has not been created");

        var permissionInfo = new PermissionInfo();
        await fixture.ManagementClient.CreatePermissionAsync(vhost, user, permissionInfo);
    }

    [Fact]
    public async Task Should_be_able_to_create_permissions_in_default_Vhost()
    {
        //create user if it does not exists
        var user = (await fixture.ManagementClient.GetUsersAsync()).SingleOrDefault(x => x.Name == TestUser)
                   ?? await fixture.ManagementClient.GetUserAsync(TestUser);

        var vhost = (await fixture.ManagementClient.GetVhostsAsync()).SingleOrDefault(x => x.Name == Vhost.Name);
        if (vhost == null)
            throw new EasyNetQTestException($"Default vhost: '{TestVHost}' has not been created");

        var permissionInfo = new PermissionInfo();
        await fixture.ManagementClient.CreatePermissionAsync(vhost, user, permissionInfo);
    }

    [Fact]
    public async Task Should_be_able_to_create_policies()
    {
        const string policyName = "asamplepolicy";
        const HaMode haMode = HaMode.All;
        const HaSyncMode haSyncMode = HaSyncMode.Automatic;
        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                Definition: new PolicyDefinition(
                    HaMode: haMode,
                    HaSyncMode: haSyncMode
                )
            )
        );
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.ApplyTo == ApplyMode.All
                 && p.Definition.HaMode == haMode
                 && p.Definition.HaSyncMode == haSyncMode)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_queues_only_policies()
    {
        const string policyName = "asamplepolicy-queue-only";
        const HaMode haMode = HaMode.All;
        const HaSyncMode haSyncMode = HaSyncMode.Automatic;
        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                ApplyTo: ApplyMode.Queues,
                Definition: new PolicyDefinition(
                    HaMode: haMode,
                    HaSyncMode: haSyncMode
                )
            )
        );
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.ApplyTo == ApplyMode.Queues
                 && p.Definition.HaMode == haMode
                 && p.Definition.HaSyncMode == haSyncMode)
        );
    }

    [Fact]
    public async Task Should_be_able_to_delete_a_queue()
    {
        var queue = await CreateTestQueue(TestQueue);
        await fixture.ManagementClient.DeleteQueueAsync(queue);
    }

    [Fact]
    public async Task Should_not_be_able_to_delete_a_non_empty_queue()
    {
        var queue = await CreateTestQueue(TestQueue);

        var publishInfo = new PublishInfo(
            TestQueue,
            "Hello World",
            PayloadEncoding.String,
            new Dictionary<string, object?>
            {
                { "app_id", "management-test" }
            }
        );

        await fixture.ManagementClient.PublishAsync("/", "amq.default", publishInfo);

        var expectedMessage = $"*\"reason\":\"queue '{TestQueue}' in vhost '{Vhost.Name}' not empty\"*";
        {
            var act = async () => await fixture.ManagementClient.DeleteQueueAsync(queue, DeleteQueueCriteria.IfEmpty);
            await act.Should().ThrowAsync<UnexpectedHttpStatusCodeException>()
                .Where(e => e.StatusCode == System.Net.HttpStatusCode.BadRequest)
                .WithMessage(expectedMessage);
        }
        {
            var act = async () => await fixture.ManagementClient.DeleteQueueAsync(queue, DeleteQueueCriteria.IfUnusedAndEmpty);
            await act.Should().ThrowAsync<UnexpectedHttpStatusCodeException>()
                .Where(e => e.StatusCode == System.Net.HttpStatusCode.BadRequest)
                .WithMessage(expectedMessage);
        }
    }

    [Fact(Skip = "Requires at least a consumer")]
    public async Task Should_not_be_able_to_delete_a_non_unused_queue()
    {
        var queue = await CreateTestQueue(TestQueue);

        // Create consumer

        var expectedMessage = $"*\"reason\":\"queue '{TestQueue}' in vhost '{Vhost.Name}' not empty\"*";
        {
            var act = async () => await fixture.ManagementClient.DeleteQueueAsync(queue, DeleteQueueCriteria.IfUnused);
            await act.Should().ThrowAsync<UnexpectedHttpStatusCodeException>()
                .Where(e => e.StatusCode == System.Net.HttpStatusCode.BadRequest)
                .WithMessage(expectedMessage);
        }
        {
            var act = async () => await fixture.ManagementClient.DeleteQueueAsync(queue, DeleteQueueCriteria.IfUnusedAndEmpty);
            await act.Should().ThrowAsync<UnexpectedHttpStatusCodeException>()
                .Where(e => e.StatusCode == System.Net.HttpStatusCode.BadRequest)
                .WithMessage(expectedMessage);
        }
    }

    [Fact]
    public async Task Should_be_able_to_delete_a_user()
    {
        var user = await fixture.ManagementClient.GetUserAsync(TestUser);
        await fixture.ManagementClient.DeleteUserAsync(user);
    }

    [Fact]
    public async Task Should_throw_when_trying_to_get_non_existant_user()
    {
        var act = async () => await fixture.ManagementClient.GetUserAsync("non-existant-user");

        await act.Should().ThrowAsync<UnexpectedHttpStatusCodeException>()
            .Where(e => e.StatusCode == System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_throw_when_trying_to_delete_non_existant_user()
    {
        var act = async () => await fixture.ManagementClient.DeleteUserAsync("non-existant-user");

        await act.Should().ThrowAsync<UnexpectedHttpStatusCodeException>()
            .Where(e => e.StatusCode == System.Net.HttpStatusCode.NotFound);
    }

    async Task AssertUserLimits(string testUser1, string testUser2, UserLimits[] expectedUserLimits)
    {
        IReadOnlyList<UserLimits> limits;
        limits = await fixture.ManagementClient.GetUserLimitsAsync(testUser1);
        limits.Should().Equal(expectedUserLimits.Where(limits => limits != null && limits.User == testUser1));
        limits = await fixture.ManagementClient.GetUserLimitsAsync(testUser2);
        limits.Should().Equal(expectedUserLimits.Where(limits => limits != null && limits.User == testUser2));
        limits = await fixture.ManagementClient.GetUserLimitsAsync();
        limits.Should().Equal(expectedUserLimits.Where(limits => limits != null));
    }

    [Fact]
    public async Task Should_be_able_to_create_get_and_delete_a_user_limits()
    {
        var testUser1 = "limits1";
        await fixture.ManagementClient.CreateUserAsync(testUser1, UserInfo.ByPassword("topSecret"));

        var testUser2 = "limits2";
        await fixture.ManagementClient.CreateUserAsync(testUser2, UserInfo.ByPassword("topSecret"));

        UserLimits[] expectedUserLimits = { null, null };
        await AssertUserLimits(testUser1, testUser2, expectedUserLimits);

        expectedUserLimits[0] = new UserLimits(testUser1, new Limits(null, 22));
        await fixture.ManagementClient.CreateUserMaxConnectionsLimitAsync(testUser1, 22);
        await AssertUserLimits(testUser1, testUser2, expectedUserLimits);

        expectedUserLimits[0] = new UserLimits(testUser1, new Limits(33, 22));
        await fixture.ManagementClient.CreateUserMaxChannelsLimitAsync(testUser1, 33);
        await AssertUserLimits(testUser1, testUser2, expectedUserLimits);

        expectedUserLimits[1] = new UserLimits(testUser2, new Limits(33, null));
        await fixture.ManagementClient.CreateUserMaxChannelsLimitAsync(testUser2, 33);
        await AssertUserLimits(testUser1, testUser2, expectedUserLimits);


        var user = await fixture.ManagementClient.GetUserAsync(testUser1);
        user.Limits.Should().BeEquivalentTo(expectedUserLimits[0].Limits);


        expectedUserLimits[1] = null;
        await fixture.ManagementClient.DeleteUserMaxChannelsLimitAsync(testUser2);
        await AssertUserLimits(testUser1, testUser2, expectedUserLimits);

        expectedUserLimits[0] = new UserLimits(testUser1, new Limits(null, 22));
        await fixture.ManagementClient.DeleteUserMaxChannelsLimitAsync(testUser1);
        await AssertUserLimits(testUser1, testUser2, expectedUserLimits);

        expectedUserLimits[0] = null;
        await fixture.ManagementClient.DeleteUserMaxConnectionsLimitAsync(testUser1);
        await AssertUserLimits(testUser1, testUser2, expectedUserLimits);
    }

    [Fact]
    public async Task Should_be_able_to_delete_an_exchange()
    {
        const string exchangeName = "delete-xcg";
        var exchange = await CreateExchange(exchangeName);

        await fixture.ManagementClient.DeleteExchangeAsync(exchange);
    }

    [Fact]
    public async Task Should_be_able_to_delete_an_exchange_with_pluses()
    {
        var exchange = await CreateExchange(TestExchangeTestQueueWithPlusChar);
        await fixture.ManagementClient.DeleteExchangeAsync(exchange);
    }

    [Fact]
    public async Task Should_not_be_able_to_delete_a_non_unused_exchange()
    {
        var queue = await CreateTestQueue(TestQueue);
        var exchange = await CreateExchange(TestExchange);

        var bindingInfo = new BindingInfo(RoutingKey: TestQueue);

        await fixture.ManagementClient.CreateQueueBindingAsync(exchange, queue, bindingInfo);

        var expectedMessage = $"*\"reason\":\"exchange '{TestExchange}' in vhost '{Vhost.Name}' in use\"*";
        {
            var act = async () => await fixture.ManagementClient.DeleteExchangeAsync(exchange, DeleteExchangeCriteria.IfUnused);
            await act.Should().ThrowAsync<UnexpectedHttpStatusCodeException>()
                .Where(e => e.StatusCode == System.Net.HttpStatusCode.BadRequest)
                .WithMessage(expectedMessage);
        }
    }

    [Fact]
    public async Task Should_be_able_to_delete_permissions()
    {
        var userInfo = UserInfo.ByPassword("topSecret").AddTag(UserTags.Monitoring).AddTag(UserTags.Management);
        await fixture.ManagementClient.CreateUserAsync(TestUser, userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(TestUser);
        await fixture.ManagementClient.CreateVhostAsync(TestVHost);
        var vhost = await fixture.ManagementClient.GetVhostAsync(TestVHost);
        var permissionInfo = new PermissionInfo();
        await fixture.ManagementClient.CreatePermissionAsync(vhost, user, permissionInfo);

        var permission = (await fixture.ManagementClient.GetPermissionsAsync())
            .SingleOrDefault(x => x.User == TestUser && x.Vhost == TestVHost);

        if (permission == null)
            throw new EasyNetQTestException($"No permission for vhost: {TestVHost} and user: {TestUser}");

        await fixture.ManagementClient.DeletePermissionAsync(permission);
    }

    [Fact]
    public async Task Should_be_able_to_delete_policies()
    {
        const string policyName = "asamplepolicy";
        await fixture.ManagementClient.CreatePolicyAsync(
            new Policy(
                Name: policyName,
                Pattern: "averyuncommonpattern",
                Vhost: Vhost.Name,
                Definition: new PolicyDefinition(
                    HaMode: HaMode.All,
                    HaSyncMode: HaSyncMode.Automatic
                )
            )
        );
        Assert.Equal(
            1,
            (await fixture.ManagementClient.GetPoliciesAsync()).Count(p => p.Name == policyName && p.Vhost == Vhost.Name)
        );
        await fixture.ManagementClient.DeletePolicyAsync(Vhost.Name, policyName);
        Assert.Equal(
            0,
            (await fixture.ManagementClient.GetPoliciesAsync()).Count(p => p.Name == policyName && p.Vhost == Vhost.Name)
        );
    }

    [Fact]
    public async Task Should_be_able_to_get_a_list_of_bindings_between_an_exchange_and_a_queue()
    {
        var queue = await EnsureQueueExists(TestQueue);

        var exchange = await EnsureExchangeExists(TestExchange);

        var bindings = (await fixture.ManagementClient.GetQueueBindingsAsync(exchange, queue)).ToArray();

        bindings.Length.Should().Be(0);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_list_of_bindings_between_an_exchange_and_an_exchange()
    {
        var exchange1 = await EnsureExchangeExists(TestExchange);
        var exchange2 = await EnsureExchangeExists(TestExchange2);

        var bindings = await fixture.ManagementClient.GetExchangeBindingsAsync(exchange1, exchange2);

        bindings.Count.Should().Be(0);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_queue_by_name()
    {
        await CreateTestQueue(TestQueue);
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, TestQueue);
        queue.Name.Should().Be(TestQueue);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_queue_by_name_with_all_detailed_information()
    {
        const int age = 10;
        const int increment = 1;
        await CreateTestQueue(TestQueue);
        await Task.Delay(TimeSpan.FromSeconds(10));

        var queue = await fixture.ManagementClient.GetQueueAsync(
            Vhost,
            TestQueue,
            new LengthsCriteria(age, increment),
            new RatesCriteria(age, increment)
        );

        queue.Name.Should().Be(TestQueue);
        queue.MessagesDetails.Should().NotBeNull();
        queue.MessagesDetails!.Samples.Should().NotBeNullOrEmpty();
        queue.MessagesReadyDetails.Should().NotBeNull();
        queue.MessagesReadyDetails!.Samples.Should().NotBeNullOrEmpty();
        queue.MessagesUnacknowledgedDetails.Should().NotBeNull();
        queue.MessagesUnacknowledgedDetails!.Samples.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_be_able_to_get_a_queue_by_name_with_detailed_length_information()
    {
        const int age = 10;
        const int increment = 1;
        await CreateTestQueue(TestQueue);
        await Task.Delay(TimeSpan.FromSeconds(10));

        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, TestQueue, new LengthsCriteria(age, increment));

        queue.Name.Should().Be(TestQueue);
        queue.MessagesDetails.Should().NotBeNull();
        queue.MessagesDetails!.Samples.Should().NotBeNullOrEmpty();
        queue.MessagesReadyDetails.Should().NotBeNull();
        queue.MessagesReadyDetails!.Samples.Should().NotBeNullOrEmpty();
        queue.MessagesUnacknowledgedDetails.Should().NotBeNull();
        queue.MessagesUnacknowledgedDetails!.Samples.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_be_able_to_get_a_queue_by_name_with_detailed_rates_information()
    {
        const int age = 60;
        const int increment = 10;
        await CreateTestQueue(TestQueue);
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, TestQueue, ratesCriteria: new RatesCriteria(age, increment));
        queue.Name.Should().Be(TestQueue);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_queue_by_name_with_plus_char()
    {
        await CreateTestQueue(TestQueueWithPlusChar);
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, TestQueueWithPlusChar);
        queue.Name.Should().Be(TestQueueWithPlusChar);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_user_by_name()
    {
        var userInfo = UserInfo.ByPassword("topSecret");

        await fixture.ManagementClient.CreateUserAsync(TestUser, userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(TestUser);

        user.Name.Should().Be(TestUser);
        user.Should().BeEquivalentTo(userInfo with { HashingAlgorithm = HashAlgorithmName.SHA256 },
            options => options.ExcludingMissingMembers().Excluding(o => o.PasswordHash));
    }

    [Fact]
    public async Task Should_be_able_to_get_all_the_bindings_for_a_queue()
    {
        var queue = await CreateTestQueue(TestQueue);
        var bindings = await fixture.ManagementClient.GetBindingsForQueueAsync(queue);
        Assert.NotEmpty(bindings.ToList());
    }

    [Fact]
    public async Task Should_be_able_to_get_an_individual_exchange_by_name()
    {
        var vhost = new Vhost(Name: Vhost.Name);
        var exchange = await fixture.ManagementClient.GetExchangeAsync(vhost, TestExchange);

        exchange.Name.Should().Be(TestExchange);
    }

    [Fact]
    public async Task Should_be_able_to_get_an_individual_vhost()
    {
        await fixture.ManagementClient.CreateVhostAsync(TestVHost);
        var vhost = await fixture.ManagementClient.GetVhostAsync(TestVHost);
        vhost.Name.Should().Be(TestVHost);
    }

    [Fact]
    public async Task Should_be_able_to_get_messages_from_a_queue()
    {
        var queue = await CreateTestQueue(TestQueue);

        var publishInfo = new PublishInfo(
            TestQueue,
            "Hello World",
            PayloadEncoding.String,
            new Dictionary<string, object?>
            {
                { "app_id", "management-test" }
            }
        );

        await fixture.ManagementClient.PublishAsync("/", "amq.default", publishInfo);

        var messages = await fixture.ManagementClient.GetMessagesFromQueueAsync(queue, new GetMessagesFromQueueInfo(1, AckMode.AckRequeueFalse));
        foreach (var message in messages)
        {
            output.WriteLine("message.Payload = {0}", message.Payload);
            foreach (var property in message.Properties)
                output.WriteLine("key: '{0}', value: '{1}'", property.Key, property.Value);
        }
    }

    [Fact]
    public void Should_be_able_to_get_policies_list()
    {
        var policies = fixture.ManagementClient.GetPoliciesAsync();
        Assert.NotNull(policies);
    }

    [Fact]
    public async Task Should_be_able_to_list_parameters()
    {
        var parameters = await fixture.ManagementClient.GetParametersAsync();
        Assert.NotNull(parameters);
    }

    [Fact]
    public async Task Should_be_able_to_publish_to_an_exchange()
    {
        var exchange = await CreateExchange(TestExchange);

        var publishInfo = new PublishInfo(TestQueue, "Hello World");
        var result = await fixture.ManagementClient.PublishAsync(exchange, publishInfo);

        // the testExchange isn't bound to a queue
        result.Routed.Should().BeFalse();
    }

    [Fact]
    public async Task Should_check_that_the_broker_is_alive()
    {
        await fixture.ManagementClient.CreateVhostAsync(TestVHost);
        var vhost = await fixture.ManagementClient.GetVhostAsync(TestVHost);
        var user = (await fixture.ManagementClient.GetUsersAsync()).Single(x => x.Name == fixture.User);
        var permissionInfo = new PermissionInfo();
        await fixture.ManagementClient.CreatePermissionAsync(vhost, user, permissionInfo);
        (await fixture.ManagementClient.IsAliveAsync(vhost)).Should().BeTrue();
    }

    [Fact]
    public async Task Should_create_a_virtual_host()
    {
        await fixture.ManagementClient.CreateVhostAsync(TestVHost);
        var vhost = await fixture.ManagementClient.GetVhostAsync(TestVHost);
        vhost.Name.Should().Be(TestVHost);
    }

    [Fact]
    public async Task Should_create_binding()
    {
        var queue = await CreateTestQueue(TestQueue);
        var exchange = await CreateExchange(TestExchange);

        var bindingInfo = new BindingInfo(RoutingKey: TestQueue);

        await fixture.ManagementClient.CreateQueueBindingAsync(exchange, queue, bindingInfo);
    }

    [Fact]
    public async Task Should_create_exchange_to_exchange_binding()
    {
        const string sourceExchangeName = "management_api_test_source_exchange";
        const string destinationExchangeName = "management_api_test_destination_exchange";

        var vhost = await fixture.ManagementClient.GetVhostAsync(Vhost.Name);
        var sourceExchangeInfo = new ExchangeInfo("direct");
        var destinationExchangeInfo = new ExchangeInfo("direct");

        await fixture.ManagementClient.CreateExchangeAsync(vhost, sourceExchangeName, sourceExchangeInfo);
        var sourceExchange = await fixture.ManagementClient.GetExchangeAsync(vhost, sourceExchangeName);

        await fixture.ManagementClient.CreateExchangeAsync(vhost, destinationExchangeName, destinationExchangeInfo);
        var destinationExchange = await fixture.ManagementClient.GetExchangeAsync(vhost, destinationExchangeName);

        await fixture.ManagementClient.CreateExchangeBindingAsync(sourceExchange, destinationExchange, new BindingInfo(RoutingKey: "#"));

        var binding = (await fixture.ManagementClient.GetBindingsWithSourceAsync(sourceExchange))[0];

        await fixture.ManagementClient.DeleteExchangeAsync(sourceExchange);
        await fixture.ManagementClient.DeleteExchangeAsync(destinationExchange);

        Assert.Equal("exchange", binding.DestinationType);
        Assert.Equal(destinationExchangeName, binding.Destination);
        Assert.Equal("#", binding.RoutingKey);
    }

    [Fact]
    public async Task Should_delete_a_virtual_host()
    {
        var vhost = await fixture.ManagementClient.GetVhostAsync(TestVHost);
        await fixture.ManagementClient.DeleteVhostAsync(vhost);
    }

    [Fact]
    public async Task Should_delete_binding()
    {
        var sourceXchange = await CreateExchange("sourceXcg");
        var queue = await CreateTestQueue(TestQueue);
        var bindingInfo = new BindingInfo(RoutingKey: "#");
        await fixture.ManagementClient.CreateQueueBindingAsync(sourceXchange, queue, bindingInfo);
        var binding = (await fixture.ManagementClient.GetQueueBindingsAsync(sourceXchange, queue))[0];
        await fixture.ManagementClient.DeleteBindingAsync(binding);
        await fixture.ManagementClient.DeleteExchangeAsync(sourceXchange);
        await fixture.ManagementClient.DeleteQueueAsync(queue);
    }

    [Fact]
    public async Task Should_delete_exchange_to_exchange_binding()
    {
        const string sourceExchangeName = "management_api_test_source_exchange";
        const string destinationExchangeName = "management_api_test_destination_exchange";

        var vhost = await fixture.ManagementClient.GetVhostAsync(Vhost.Name);
        var sourceExchangeInfo = new ExchangeInfo("direct");
        var destinationExchangeInfo = new ExchangeInfo("direct");

        await fixture.ManagementClient.CreateExchangeAsync(vhost, sourceExchangeName, sourceExchangeInfo);
        var sourceExchange = await fixture.ManagementClient.GetExchangeAsync(vhost, sourceExchangeName);

        await fixture.ManagementClient.CreateExchangeAsync(vhost, destinationExchangeName, destinationExchangeInfo);
        var destinationExchange = await fixture.ManagementClient.GetExchangeAsync(vhost, destinationExchangeName);

        await fixture.ManagementClient.CreateExchangeBindingAsync(sourceExchange, destinationExchange, new BindingInfo(RoutingKey: "#"));

        var binding = (await fixture.ManagementClient.GetExchangeBindingsAsync(sourceExchange, destinationExchange))[0];

        await fixture.ManagementClient.DeleteBindingAsync(binding);
        await fixture.ManagementClient.DeleteExchangeAsync(sourceExchange);
        await fixture.ManagementClient.DeleteExchangeAsync(destinationExchange);
    }

    [Fact]
    public async Task Should_disable_tracing()
    {
        var vhost = await fixture.ManagementClient.GetVhostAsync(Vhost.Name);
        await fixture.ManagementClient.DisableTracingAsync(vhost);
        var vhostAfterUpdate = await fixture.ManagementClient.GetVhostAsync(Vhost.Name);
        Assert.False(vhostAfterUpdate.Tracing);
    }

    [Fact]
    public async Task Should_enable_tracing()
    {
        var vhost = await fixture.ManagementClient.GetVhostAsync(Vhost.Name);
        await fixture.ManagementClient.EnableTracingAsync(vhost);
        var vhostAfterUpdate = await fixture.ManagementClient.GetVhostAsync(Vhost.Name);
        Assert.True(vhostAfterUpdate.Tracing);
    }

    [Fact]
    public async Task Should_get_all_bindings_for_which_the_exchange_is_the_destination()
    {
        var sourceXchange = await CreateExchange("sourceXcg");
        var destinationXchange = await CreateExchange("destinationXcg");
        var bindingInfo = new BindingInfo(RoutingKey: "#");
        await fixture.ManagementClient.CreateExchangeBindingAsync(sourceXchange, destinationXchange, bindingInfo);

        Assert.NotEmpty((await fixture.ManagementClient.GetBindingsWithDestinationAsync(destinationXchange)).ToList());
    }

    [Fact]
    public async Task Should_get_all_bindings_for_which_the_exchange_is_the_source()
    {
        var sourceXchange = await CreateExchange("sourceXcg");
        var destinationXchange = await CreateExchange("destinationXcg");
        var bindingInfo = new BindingInfo(RoutingKey: "#");
        await fixture.ManagementClient.CreateExchangeBindingAsync(sourceXchange, destinationXchange, bindingInfo);
        Assert.NotEmpty((await fixture.ManagementClient.GetBindingsWithSourceAsync(sourceXchange)).ToList());
    }

    [Fact]
    public async Task Should_get_bindings()
    {
        foreach (var binding in await fixture.ManagementClient.GetBindingsAsync())
        {
            output.WriteLine("binding.Destination = {0}", binding.Destination);
            output.WriteLine("binding.Source = {0}", binding.Source);
            output.WriteLine("binding.PropertiesKey = {0}", binding.PropertiesKey);
        }
    }

    [Fact(Skip = "Requires at least a consumer")]
    public async Task Should_get_consumers()
    {
        foreach (var consumer in await fixture.ManagementClient.GetConsumersAsync())
        {
            output.WriteLine("consumer.ConsumerTag = {0}", consumer.ConsumerTag);
            consumer.ChannelDetails.Should().NotBeNull();
            if (consumer.ChannelDetails != null)
            {
                output.WriteLine("consumer.ChannelDetails.ConnectionName = {0}", consumer.ChannelDetails.ConnectionName);
                output.WriteLine("consumer.ChannelDetails.ConnectionName = {0}", consumer.ChannelDetails.ConnectionName);
                output.WriteLine("consumer.ChannelDetails.Node = {0}", consumer.ChannelDetails.Node);
            }
        }
    }

    [Fact]
    public async Task Should_get_channels()
    {
        var channels = await fixture.ManagementClient.GetChannelsAsync();

        foreach (var channel in channels)
        {
            output.WriteLine("channel.Name = {0}", channel.Name);
            output.WriteLine("channel.User = {0}", channel.User);
            output.WriteLine("channel.PrefetchCount = {0}", channel.PrefetchCount);
        }
    }

    [Fact]
    public async Task Should_get_channels_per_connection()
    {
        using var bus = RabbitHutch.CreateBus("host=localhost;publisherConfirms=True");

        await Task.Delay(TimeSpan.FromSeconds(10));

        await bus.Advanced.ConnectAsync();
        await bus.Advanced.PublishAsync(Topology.Exchange.Default, "#", false, new MessageProperties(), ReadOnlyMemory<byte>.Empty);

        await Task.Delay(TimeSpan.FromSeconds(10));

        var connections = await fixture.ManagementClient.GetConnectionsAsync();
        foreach (var connection in connections)
        {
            output.WriteLine("connection.Name = {0}", connection.Name);
            var channels = await fixture.ManagementClient.GetChannelsAsync(connection);

            foreach (var channel in channels) output.WriteLine("\tchannel.Name = {0}", channel.Name);
        }
    }

    [Fact]
    public async Task Should_get_connections()
    {
        using var bus = RabbitHutch.CreateBus("host=localhost;publisherConfirms=True");

        await Task.Delay(TimeSpan.FromSeconds(10));

        await bus.Advanced.ConnectAsync();

        foreach (var connection in await fixture.ManagementClient.GetConnectionsAsync())
        {
            output.WriteLine("Connection.Name: {0}", connection.Name);
            output.WriteLine("Connection.ClientProperties: {0}", connection.ClientProperties);
        }
    }

    [Fact]
    public async Task Should_get_definitions()
    {
        var definitions = await fixture.ManagementClient.GetDefinitionsAsync();

        definitions.RabbitVersion[0].Should().Be('3');
    }

    [Fact]
    public async Task Should_get_exchanges()
    {
        var exchanges = await fixture.ManagementClient.GetExchangesAsync();

        foreach (var exchange in exchanges) output.WriteLine("exchange.Name = {0}", exchange.Name);
    }

    [Fact]
    public async Task Should_get_exchanges_with_pagination()
    {
        var page = await fixture.ManagementClient.GetExchangesByPageAsync(new PageCriteria(1, 7, "amq"));

        page.Items.Count.Should().BeGreaterThan(0);
    }

    [Fact(Skip = "Requires at least an active federation")]
    public async Task Should_get_federations()
    {
        var federations = await fixture.ManagementClient.GetFederationsAsync();

        federations.Single().Node.Should().Be($"rabbit@{fixture.Endpoint}");
    }

    [Fact]
    public async Task Should_get_nodes()
    {
        var nodes = (await fixture.ManagementClient.GetNodesAsync()).ToList();

        nodes.Count.Should().NotBe(0);
        nodes[0].Name.Should().Be("rabbit@easynetq");
    }

    [Fact]
    public async Task Should_get_overview()
    {
        using var bus = RabbitHutch.CreateBus("host=localhost;publisherConfirms=True");

        await Task.Delay(TimeSpan.FromSeconds(10));

        await bus.Advanced.ConnectAsync();

        var overview = await fixture.ManagementClient.GetOverviewAsync();

        output.WriteLine("overview.ManagementVersion = {0}", overview.ManagementVersion);
        foreach (var exchangeType in overview.ExchangeTypes)
            output.WriteLine("exchangeType.Name = {0}", exchangeType.Name);
        foreach (var listener in overview.Listeners)
            output.WriteLine("listener.IpAddress = {0}", listener.IpAddress);

        output.WriteLine("overview.Messages = {0}", overview.QueueTotals?.Messages ?? 0);

        foreach (var context in overview.Contexts)
            output.WriteLine("context.Description = {0}", context.Description);
    }

    [Fact]
    public async Task Should_get_permissions()
    {
        var permissions = await fixture.ManagementClient.GetPermissionsAsync();

        foreach (var permission in permissions)
        {
            output.WriteLine("permission.User = {0}", permission.User);
            output.WriteLine("permission.Vhost = {0}", permission.Vhost);
            output.WriteLine("permission.Configure = {0}", permission.Configure);
            output.WriteLine("permission.Read = {0}", permission.Read);
            output.WriteLine("permission.Write = {0}", permission.Write);
        }
    }

    [Fact]
    public async Task Should_get_queues()
    {
        await CreateTestQueue(TestQueue);
        var act = async () =>
            {
                var queues = await fixture.ManagementClient.GetQueuesAsync();
                queues.Should().NotBeNullOrEmpty()
                    .And.AllSatisfy(q =>
                        {
                            q.State.Should().Be("running");
                            // Should be not null only because some keys in response aren't mapped to Queue's properties.
                            // E.g. message_bytes_ready or operator_policy
                            q.ExtensionData.Should().NotBeNull();
                        })
                    .And.ContainEquivalentOf(new QueueName(TestQueue, Vhost.Name));
            };

        await act.Should().NotThrowAfterAsync(10.Seconds(), TimeSpan.Zero);
    }

    [Fact]
    public async Task Should_get_queues_with_stats_criteria()
    {
        var queue = await CreateTestQueue(TestQueue);

        var publishInfo = new PublishInfo(
            TestQueue,
            "Hello World",
            PayloadEncoding.String,
            new Dictionary<string, object?>
            {
                { "app_id", "management-test" }
            }
        );

        await fixture.ManagementClient.PublishAsync("/", "amq.default", publishInfo);
        Queue queueWithStats;
        do
        {
            queueWithStats = (await fixture.ManagementClient.GetQueuesAsync()).Single(q => q.Name == TestQueue);
        } while (queueWithStats.Messages == 0);
        queueWithStats.Messages.Should().Be(1);
        queueWithStats.MessagesDetails.Should().NotBeNull();
        queueWithStats.MessageStats.Should().NotBeNull();

        var queueWithoutStats = (await fixture.ManagementClient.GetQueuesAsync(StatsCriteria.Disable)).Single(q => q.Name == TestQueue);
        queueWithoutStats.Messages.Should().Be(0);
        queueWithoutStats.MessagesDetails.Should().BeNull();
        queueWithoutStats.MessageStats.Should().BeNull();

        var queueWithTotalsOnly = (await fixture.ManagementClient.GetQueuesAsync(StatsCriteria.QueueTotalsOnly)).Single(q => q.Name == TestQueue);
        queueWithTotalsOnly.Messages.Should().Be(1);
        queueWithTotalsOnly.MessagesDetails.Should().BeNull();
        queueWithTotalsOnly.MessageStats.Should().BeNull();
    }


    [Fact]
    public async Task Should_get_queues_with_pagination()
    {
        foreach (var queue in await fixture.ManagementClient.GetQueuesAsync())
            await fixture.ManagementClient.DeleteQueueAsync(queue);

        await CreateTestQueue("1");
        await CreateTestQueue("2");

        var firstPage = await fixture.ManagementClient.GetQueuesByPageAsync(new PageCriteria(1, 1));
        firstPage.Items.Count.Should().Be(1);

        var secondPage = await fixture.ManagementClient.GetQueuesByPageAsync(new PageCriteria(2, 1));
        secondPage.Items.Count.Should().Be(1);
    }

    [Fact]
    public async Task Should_get_queues_by_vhost()
    {
        await fixture.ManagementClient.CreateVhostAsync(TestVHost);
        var vhost = await fixture.ManagementClient.GetVhostAsync(TestVHost);
        vhost.Name.Should().Be(TestVHost);

        await CreateTestQueueInVhost($"{TestVHost}_{TestQueue}", vhost);
        (await fixture.ManagementClient.GetQueuesAsync(vhost)).Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_get_users()
    {
        var users = await fixture.ManagementClient.GetUsersAsync();

        foreach (var user in users) output.WriteLine("user.Name = {0}", user.Name);
    }

    [Fact]
    public async Task Should_get_vhosts()
    {
        var vhosts = await fixture.ManagementClient.GetVhostsAsync();

        foreach (var vhost in vhosts) output.WriteLine("vhost.Name = {0}", vhost.Name);
    }

    [Fact]
    public async Task Should_purge_a_queue()
    {
        var queue = await CreateTestQueue(TestQueue);
        await fixture.ManagementClient.PurgeAsync(queue);
    }

    [Fact]
    public async Task Should_throw_when_trying_to_close_unknown_connection()
    {
        var act = async () => await fixture.ManagementClient.CloseConnectionAsync("unknown");

        await act.Should().ThrowAsync<UnexpectedHttpStatusCodeException>()
            .Where(e => e.StatusCode == System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_be_able_to_create_shovel_parameter_on_queue()
    {
        var srcUri = new AmqpUri(fixture.Endpoint.Host, fixture.Endpoint.Port, fixture.User, fixture.Password);
        var destUri = new AmqpUri($"{fixture.Endpoint.Host}-1", fixture.Endpoint.Port, fixture.User, fixture.Password);

        var shovelName = "queue-shovel";
        var parameterShovelValue = new ParameterShovelValue
            (
                SrcProtocol: AmqpProtocol.AMQP091,
                SrcUri: srcUri.ToString(),
                SrcQueue: "test-queue-src",
                SrcQueueArguments: new Dictionary<string, object?> { { "x-queue-mode", "lazy" } },
                SrcDeleteAfter: "never",
                SrcPrefetchCount: 10,
                DestProtocol: AmqpProtocol.AMQP091,
                DestUri: destUri.ToString(),
                DestQueue: "test-queue-dest",
                DestQueueArguments: new Dictionary<string, object?> { { "x-queue-mode", "lazy" } },
                DestAddForwardHeaders: false,
                DestAddTimestampHeader: false,
                AckMode: "on-confirm",
                ReconnectDelay: 10
            );

        await fixture.ManagementClient.CreateShovelAsync(
            vhostName: Vhost.Name,
            shovelName,
            parameterShovelValue
        );

        var parameter = await fixture.ManagementClient.GetShovelAsync(Vhost.Name, shovelName);

        Assert.Equal(shovelName, parameter.Name);
        parameter.Value.Should().BeOfType<JsonElement>();
        if (parameter.Value is JsonElement jsonElement)
        {
            var actualParameterShovelValue = JsonSerializer.Deserialize<ParameterShovelValue>(jsonElement.GetRawText());
            actualParameterShovelValue.Should().BeEquivalentTo(parameterShovelValue);
        }

        await fixture.ManagementClient.DeleteParameterAsync(
            "shovel",
            vhostName: Vhost.Name,
            shovelName
        );

        var act =
            async () => await fixture.ManagementClient.DeleteParameterAsync(
                "shovel",
                vhostName: Vhost.Name,
                shovelName
            );

        await act.Should().ThrowAsync<UnexpectedHttpStatusCodeException>()
            .Where(e => e.StatusCode == System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_be_able_to_create_shovel_parameter_on_exchange()
    {
        var srcUri = new AmqpUri(fixture.Endpoint.Host, fixture.Endpoint.Port, fixture.User, fixture.Password);
        var destUri = new AmqpUri($"{fixture.Endpoint.Host}-1", fixture.Endpoint.Port, fixture.User, fixture.Password);

        var shovelName = "exchange-shovel";
        var parameterShovelValue = new ParameterShovelValue
            (
                SrcProtocol: AmqpProtocol.AMQP091,
                SrcUri: srcUri.ToString(),
                SrcExchange: "test-exchange-src",
                SrcExchangeKey: "aaa",
                SrcDeleteAfter: "never",
                SrcPrefetchCount: 10,
                DestProtocol: AmqpProtocol.AMQP091,
                DestUri: destUri.ToString(),
                DestExchange: "test-exchange-dest",
                DestExchangeKey: "bbb",
                DestAddForwardHeaders: false,
                DestAddTimestampHeader: false,
                AckMode: "on-confirm",
                ReconnectDelay: 10
            );

        await fixture.ManagementClient.CreateShovelAsync(
            vhostName: Vhost.Name,
            shovelName,
            parameterShovelValue
        );

        var parameter = await fixture.ManagementClient.GetShovelAsync(Vhost.Name, shovelName);

        Assert.Equal(shovelName, parameter.Name);
        parameter.Value.Should().BeOfType<JsonElement>();
        if (parameter.Value is JsonElement jsonElement)
        {
            var actualParameterShovelValue = JsonSerializer.Deserialize<ParameterShovelValue>(jsonElement.GetRawText());
            actualParameterShovelValue.Should().BeEquivalentTo(parameterShovelValue);
        }
    }

    [Fact]
    public async Task Should_throw_when_trying_to_get_non_existant_shovel()
    {
        var act = async () => await fixture.ManagementClient.GetShovelAsync(Vhost.Name, "non-existant-shovel");

        await act.Should().ThrowAsync<UnexpectedHttpStatusCodeException>()
            .Where(e => e.StatusCode == System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_be_able_to_get_existant_shovel()
    {
        var srcUri = new AmqpUri(fixture.Endpoint.Host, fixture.Endpoint.Port, fixture.User, fixture.Password);
        var destUri = new AmqpUri($"{fixture.Endpoint.Host}-1", fixture.Endpoint.Port, fixture.User, fixture.Password);

        var shovelName = "exchange-shovel";

        await fixture.ManagementClient.CreateShovelAsync(
            vhostName: Vhost.Name,
            shovelName: shovelName,
            new ParameterShovelValue
            (
                SrcProtocol: AmqpProtocol.AMQP091,
                SrcUri: srcUri.ToString(),
                SrcExchange: "test-exchange-src",
                SrcExchangeKey: null,
                SrcQueue: null,
                SrcDeleteAfter: "never",
                DestProtocol: AmqpProtocol.AMQP091,
                DestUri: destUri.ToString(),
                DestExchange: "test-exchange-dest",
                DestQueue: null,
                AckMode: "on-confirm",
                DestAddForwardHeaders: false
            )
        );

        var shovel = await fixture.ManagementClient.GetShovelAsync(Vhost.Name, shovelName);
        Assert.Contains(shovelName, shovel.Name);
    }

    [Fact]
    public async Task Should_be_able_to_get_shovel_statuses()
    {
        var srcUri = new AmqpUri(fixture.Endpoint.Host, 5672, fixture.User, fixture.Password);
        var destUri = new AmqpUri(fixture.Endpoint.Host, 5672, fixture.User, fixture.Password);

        var shovelName = "exchange-shovel";
        var parameterShovelValue = new ParameterShovelValue
            (
                SrcProtocol: AmqpProtocol.AMQP091,
                SrcUri: srcUri.ToString(),
                SrcExchange: "test-exchange-src",
                SrcExchangeKey: "aaa",
                SrcDeleteAfter: "never",
                SrcPrefetchCount: 10,
                DestProtocol: AmqpProtocol.AMQP091,
                DestUri: destUri.ToString(),
                DestExchange: "test-exchange-dest",
                DestExchangeKey: "bbb",
                DestAddForwardHeaders: false,
                DestAddTimestampHeader: false,
                AckMode: "on-confirm",
                ReconnectDelay: 10
            );

        await fixture.ManagementClient.CreateShovelAsync(
            vhostName: Vhost.Name,
            shovelName,
            parameterShovelValue
        );

        while (true)
        {
            var shovelStatuses = await fixture.ManagementClient.GetShovelStatusesAsync();
            try
            {
                shovelStatuses.Should().ContainEquivalentOf(
                    new ShovelStatus(
                        Name: shovelName,
                        Vhost: Vhost.Name,
                        Node: "rabbit@easynetq",
                        Timestamp: default,
                        Type: "dynamic",
                        State: "starting"),
                    options => options.Excluding(ss => ss.Timestamp));
            }
            catch
            {
                shovelStatuses.Should().ContainEquivalentOf(
                    new ShovelStatus(
                        Name: shovelName,
                        Vhost: Vhost.Name,
                        Node: "rabbit@easynetq",
                        Timestamp: default,
                        Type: "dynamic",
                        State: "terminated",

                        Reason: "\"needed a restart\""
                    ),
                    options => options.Excluding(ss => ss.Timestamp));
                break;
            }
        }
    }

    [Fact]
    public async Task Should_be_able_to_get_shovel_statuses_by_vhost()
    {
        await fixture.ManagementClient.CreateVhostAsync(TestVHost);
        var vhost = await fixture.ManagementClient.GetVhostAsync(TestVHost);
        vhost.Name.Should().Be(TestVHost);

        var srcUri = new AmqpUri(fixture.Endpoint.Host, 5672, fixture.User, fixture.Password);
        var destUri = new AmqpUri(fixture.Endpoint.Host, 5672, fixture.User, fixture.Password);

        var shovelName = "queue-shovel-vhost";
        var parameterShovelValue = new ParameterShovelValue
            (
                SrcProtocol: AmqpProtocol.AMQP091,
                SrcUri: srcUri.ToString(),
                SrcQueue: "test-queue-src",
                SrcQueueArguments: new Dictionary<string, object?> { { "x-queue-mode", "lazy" } },
                SrcDeleteAfter: "never",
                SrcPrefetchCount: 10,
                DestProtocol: AmqpProtocol.AMQP091,
                DestUri: destUri.ToString(),
                DestQueue: "test-queue-dest",
                DestQueueArguments: new Dictionary<string, object?> { { "x-queue-mode", "lazy" } },
                DestAddForwardHeaders: false,
                DestAddTimestampHeader: false,
                AckMode: "on-confirm",
                ReconnectDelay: 10
            );

        await fixture.ManagementClient.CreateShovelAsync(
            vhostName: vhost.Name,
            shovelName,
            parameterShovelValue
        );

        while (true)
        {
            var shovelStatuses = await fixture.ManagementClient.GetShovelStatusesAsync(vhost.Name);
            try
            {
                shovelStatuses.Should().ContainEquivalentOf(
                    new ShovelStatus(
                        Name: shovelName,
                        Vhost: vhost.Name,
                        Node: "rabbit@easynetq",
                        Timestamp: default,
                        Type: "dynamic",
                        State: "starting"),
                    options => options.Excluding(ss => ss.Timestamp));
            }
            catch
            {
                shovelStatuses.Should().ContainEquivalentOf(
                    new ShovelStatus(
                        Name: shovelName,
                        Vhost: vhost.Name,
                        Node: "rabbit@easynetq",
                        Timestamp: default,
                        Type: "dynamic",
                        State: "running",

                        SrcProtocol: parameterShovelValue.SrcProtocol,
                        SrcUri: parameterShovelValue.SrcUri,
                        SrcQueue: parameterShovelValue.SrcQueue,
                        SrcExchange: parameterShovelValue.SrcExchange,
                        SrcExchangeKey: parameterShovelValue.SrcExchangeKey,
                        DestProtocol: parameterShovelValue.DestProtocol,
                        DestUri: parameterShovelValue.DestUri,
                        DestQueue: parameterShovelValue.DestQueue,
                        DestExchange: parameterShovelValue.DestExchange,
                        DestExchangeKey: parameterShovelValue.DestExchangeKey,
                        BlockedStatus: fixture.RabbitmqVersion >= new Version("3.11") ? "running" : null
                    ),
                    options => options.Excluding(ss => ss.Timestamp));
                break;
            }
        }
    }
}
