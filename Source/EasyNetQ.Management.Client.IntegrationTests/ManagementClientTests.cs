using EasyNetQ.Management.Client.Model;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.Management.Client.IntegrationTests;

[Collection("RabbitMQ")]
public class ManagementClientTests
{
    private readonly RabbitMqFixture fixture;
    private static readonly Vhost Vhost = new() { Name = "/", Tracing = false };

    public ManagementClientTests(RabbitMqFixture fixture)
    {
        this.fixture = fixture;
    }

    private const string testExchange = "management_api_test_exchange";
    private const string testExchange2 = "management_api_test_exchange2";
    private const string testExchangetestQueueWithPlusChar = "management_api_test_exchange+plus+test";
    private const string testQueue = "management_api_test_queue";
    private const string testQueueWithPlusChar = "management_api_test_queue+plus+test";
    private const string testUser = "mikey";

    private async Task<Exchange> CreateExchange(string exchangeName)
    {
        await fixture.ManagementClient.CreateExchangeAsync(new ExchangeInfo(exchangeName, "direct"), Vhost);
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
        var queueInfo = new QueueInfo(queueName);
        await fixture.ManagementClient.CreateQueueAsync(queueInfo, vhost);
        return await fixture.ManagementClient.GetQueueAsync(vhost, queueName);
    }

    private async Task<Exchange> EnsureExchangeExists(string exchangeName)
    {
        return (await fixture.ManagementClient.GetExchangesAsync())
               .SingleOrDefault(x => x.Name == exchangeName) ?? await CreateExchange(exchangeName);
    }

    private const string testVHost = "management_test_virtual_host";

    [Fact]
    public async Task Should_be_able_to_change_the_password_of_a_user()
    {
        var userInfo = new UserInfo(testUser, "topSecret").AddTag(UserTags.Monitoring).AddTag(UserTags.Management);
        await fixture.ManagementClient.CreateUserAsync(userInfo);

        var user = await fixture.ManagementClient.GetUserAsync(testUser);

        await fixture.ManagementClient.ChangeUserPasswordAsync(testUser, "newPassword");

        var updatedUser = await fixture.ManagementClient.GetUserAsync(testUser);
        updatedUser.Name.Should().Be(user.Name);
        updatedUser.Tags.Should().BeEquivalentTo(user.Tags);
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
            fixture.Host, fixture.User, fixture.Password, configureHttpRequestMessage: req => req.Headers.Add("x-not-used", "some_value")
        );

        await client.GetOverviewAsync();
    }

    [Fact]
    public async Task Should_be_able_to_create_a_queue()
    {
        await fixture.ManagementClient.CreateQueueAsync(new QueueInfo(testQueue), Vhost);
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, testQueue);

        queue.Name.Should().Be(testQueue);
    }

    [Fact]
    public async Task Should_be_able_to_create_a_queue_with_arguments()
    {
        const string exchangeName = "test-dead-letter-exchange";
        const string argumentKey = "x-dead-letter-exchange";
        var queueInfo = new QueueInfo($"{testQueue}1");
        queueInfo.Arguments.Add(argumentKey, exchangeName);
        await fixture.ManagementClient.CreateQueueAsync(queueInfo, Vhost);
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, queueInfo.Name);
        queue.Arguments[argumentKey].Should().NotBeNull();
        queue.Arguments[argumentKey].Should().Be(exchangeName);
    }

    [Fact]
    public async Task Should_be_able_to_create_a_queue_with_plus_char_in_the_name()
    {
        var queueInfo = new QueueInfo(testQueueWithPlusChar);

        await fixture.ManagementClient.CreateQueueAsync(queueInfo, Vhost);
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, testQueueWithPlusChar);

        queue.Name.Should().Be(testQueueWithPlusChar);
    }

    [Fact]
    public async Task Should_be_able_to_create_a_user()
    {
        var userInfo = new UserInfo(testUser, "topSecret").AddTag(UserTags.Administrator);

        await fixture.ManagementClient.CreateUserAsync(userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(testUser);

        user.Name.Should().Be(testUser);
        user.Tags.Should().Contain(UserTags.Administrator);
    }

    [Fact]
    public async Task Should_be_able_to_create_a_user_with_password_hash()
    {
        var testUser = "hash_user";
        // Hash calculated using RabbitMq hash computing algorithm using Sha256
        // See https://www.rabbitmq.com/passwords.html
        var passwordHash = "Qlp9Dgrqvx1S1VkuYsoWwgUD2XW2gZLuqQwreE+PAsPZETgo"; //"topSecret"
        var userInfo = new UserInfo(testUser, passwordHash, true).AddTag(UserTags.Administrator);

        await fixture.ManagementClient.CreateUserAsync(userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(testUser);

        user.Name.Should().Be(testUser);
    }

    [Fact]
    public async Task Should_be_able_to_create_a_user_with_the_policymaker_tag()
    {
        var userInfo = new UserInfo(testUser, "topSecret").AddTag(UserTags.Policymaker);

        await fixture.ManagementClient.CreateUserAsync(userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(testUser);

        user.Name.Should().Be(testUser);
    }

    [Fact]
    public async Task Should_be_able_to_create_a_user_without_password()
    {
        var testUser = "empty";
        var userInfo = new UserInfo(testUser, "", true).AddTag(UserTags.Administrator);

        await fixture.ManagementClient.CreateUserAsync(userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(testUser);

        user.Name.Should().Be(testUser);
    }

    [Fact]
    public async Task Should_be_able_to_create_all_the_defitions_in_a_policy()
    {
        const string policyName = "a-sample-all-definitions-in-a-policy";
        const int priority = 999;
        const HaMode haMode = HaMode.All;
        const HaSyncMode haSyncMode = HaSyncMode.Automatic;
        const string alternateExchange = "a-sample-alternate-exchange";
        const string deadLetterExchange = "a-sample-dead-letter-exchange";
        const string deadLetterRoutingKey = "a-sample-dead-letter-exchange-key";
        const uint messageTtl = 5000;
        const uint expires = 10000;
        const uint maxLength = 500;
        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            Definition = new PolicyDefinition
            {
                HaMode = haMode,
                HaSyncMode = haSyncMode,
                AlternateExchange = alternateExchange,
                DeadLetterExchange = deadLetterExchange,
                DeadLetterRoutingKey = deadLetterRoutingKey,
                MessageTtl = messageTtl,
                Expires = expires,
                MaxLength = maxLength
            },
            Priority = priority
        });
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Priority == priority
                 && p.Definition.HaMode == haMode
                 && p.Definition.HaSyncMode == haSyncMode
                 && p.Definition.AlternateExchange == alternateExchange
                 && p.Definition.DeadLetterExchange == deadLetterExchange
                 && p.Definition.DeadLetterRoutingKey == deadLetterRoutingKey
                 && p.Definition.MessageTtl == messageTtl
                 && p.Definition.Expires == expires
                 && p.Definition.MaxLength == maxLength)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_alternate_exchange_policy()
    {
        const string policyName = "a-sample-alternate-exchange-policy";
        const string alternateExchange = "a-sample-alternate-exchange";
        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            Definition = new PolicyDefinition
            {
                AlternateExchange = alternateExchange
            }
        });
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Definition.AlternateExchange == alternateExchange)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_an_exchange()
    {
        var exchange = await CreateExchange(testExchange);
        exchange.Name.Should().Be(testExchange);
    }

    [Fact]
    public async Task Should_be_able_to_create_an_exchange_with_plus_char_in_the_name()
    {
        var exchangeInfo = new ExchangeInfo(testExchangetestQueueWithPlusChar, "direct");

        await fixture.ManagementClient.CreateExchangeAsync(exchangeInfo, Vhost);
        var exchange = await fixture.ManagementClient.GetExchangeAsync(Vhost, testExchangetestQueueWithPlusChar);

        exchange.Name.Should().Be(testExchangetestQueueWithPlusChar);
    }

    [Fact]
    public async Task Should_be_able_to_create_dead_letter_exchange_policy()
    {
        const string policyName = "a-sample-dead-letter-exchange";
        const string deadLetterExchange = "a-sample-dead-letter-exchange";
        const string deadLetterRoutingKey = "a-sample-dead-letter-exchange-key";
        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            Definition = new PolicyDefinition
            {
                DeadLetterExchange = deadLetterExchange,
                DeadLetterRoutingKey = deadLetterRoutingKey
            }
        });
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

        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            ApplyTo = ApplyMode.Exchanges,
            Definition = new PolicyDefinition
            {
                HaMode = haMode,
                HaSyncMode = haSyncMode
            }
        });
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
        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            Definition = new PolicyDefinition
            {
                Expires = expires
            }
        });
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

        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            Definition = new PolicyDefinition
            {
                FederationUpstream = "my-upstream"
            }
        });
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

        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            Definition = new PolicyDefinition
            {
                FederationUpstreamSet = "my-upstream-set"
            }
        });
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
        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            Definition = new PolicyDefinition
            {
                MaxLength = maxLength
            }
        });
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Definition.MaxLength == maxLength)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_message_ttl_policy()
    {
        const string policyName = "a-sample-message-ttl";
        const uint messageTtl = 5000;
        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            Definition = new PolicyDefinition
            {
                MessageTtl = messageTtl
            }
        });
        Assert.Equal(1, (await fixture.ManagementClient.GetPoliciesAsync()).Count(
            p => p.Name == policyName
                 && p.Vhost == Vhost.Name
                 && p.Definition.MessageTtl == messageTtl)
        );
    }

    [Fact]
    public async Task Should_be_able_to_create_parameter()
    {
        await fixture.ManagementClient.CreateParameterAsync(new Parameter
        {
            Component = "federation-upstream",
            Name = "myfakefederationupstream1",
            Vhost = Vhost.Name,
            Value = new { value = new { uri = $"amqp://{fixture.User}:{fixture.Password}@{fixture.Host}" } }
        });
        Assert.Contains(await fixture.ManagementClient.GetParametersAsync(), p => p.Name == "myfakefederationupstream1");
    }

    [Fact]
    public async Task Should_be_able_to_create_permissions()
    {
        var user = (await fixture.ManagementClient.GetUsersAsync()).SingleOrDefault(x => x.Name == testUser);
        if (user == null)
            throw new EasyNetQTestException($"user '{testUser}' hasn't been created");

        var vhost = (await fixture.ManagementClient.GetVhostsAsync()).SingleOrDefault(x => x.Name == testVHost);
        if (vhost == null)
            throw new EasyNetQTestException($"Test vhost: '{testVHost}' has not been created");

        var permissionInfo = new PermissionInfo(user.Name);
        await fixture.ManagementClient.CreatePermissionAsync(vhost, permissionInfo);
    }

    [Fact]
    public async Task Should_be_able_to_create_permissions_in_default_Vhost()
    {
        var user = (await fixture.ManagementClient.GetUsersAsync()).SingleOrDefault(x => x.Name == testUser);
        if (user == null)
        {
            //create user if it does not exists
            var userInfo = new UserInfo(testUser, "topSecret").AddTag(UserTags.Administrator);
            user = await fixture.ManagementClient.GetUserAsync(testUser);
        }

        var vhost = (await fixture.ManagementClient.GetVhostsAsync()).SingleOrDefault(x => x.Name == Vhost.Name);
        if (vhost == null)
            throw new EasyNetQTestException($"Default vhost: '{testVHost}' has not been created");

        var permissionInfo = new PermissionInfo(user.Name);
        await fixture.ManagementClient.CreatePermissionAsync(vhost, permissionInfo);
    }

    [Fact]
    public async Task Should_be_able_to_create_policies()
    {
        const string policyName = "asamplepolicy";
        const HaMode haMode = HaMode.All;
        const HaSyncMode haSyncMode = HaSyncMode.Automatic;
        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            Definition = new PolicyDefinition
            {
                HaMode = haMode,
                HaSyncMode = haSyncMode
            }
        });
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
        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            ApplyTo = ApplyMode.Queues,
            Definition = new PolicyDefinition
            {
                HaMode = haMode,
                HaSyncMode = haSyncMode
            }
        });
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
        var queue = await CreateTestQueue(testQueue);
        await fixture.ManagementClient.DeleteQueueAsync(queue);
    }

    [Fact]
    public async Task Should_be_able_to_delete_a_user()
    {
        var user = await fixture.ManagementClient.GetUserAsync(testUser);
        await fixture.ManagementClient.DeleteUserAsync(user);
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
        var exchange = await CreateExchange(testExchangetestQueueWithPlusChar);
        await fixture.ManagementClient.DeleteExchangeAsync(exchange);
    }

    [Fact]
    public async Task Should_be_able_to_delete_permissions()
    {
        var userInfo = new UserInfo(testUser, "topSecret").AddTag(UserTags.Monitoring).AddTag(UserTags.Management);
        await fixture.ManagementClient.CreateUserAsync(userInfo);
        var user = await fixture.ManagementClient.GetUserAsync(testUser);
        await fixture.ManagementClient.CreateVhostAsync(testVHost);
        var vhost = await fixture.ManagementClient.GetVhostAsync(testVHost);
        var permissionInfo = new PermissionInfo(user.Name);
        await fixture.ManagementClient.CreatePermissionAsync(vhost, permissionInfo);

        var permission = (await fixture.ManagementClient.GetPermissionsAsync())
            .SingleOrDefault(x => x.User == testUser && x.Vhost == testVHost);

        if (permission == null)
            throw new EasyNetQTestException($"No permission for vhost: {testVHost} and user: {testUser}");

        await fixture.ManagementClient.DeletePermissionAsync(permission);
    }

    [Fact]
    public async Task Should_be_able_to_delete_policies()
    {
        const string policyName = "asamplepolicy";
        await fixture.ManagementClient.CreatePolicyAsync(new Policy
        {
            Name = policyName,
            Pattern = "averyuncommonpattern",
            Vhost = Vhost.Name,
            Definition = new PolicyDefinition
            {
                HaMode = HaMode.All,
                HaSyncMode = HaSyncMode.Automatic
            }
        });
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
        var queue = await EnsureQueueExists(testQueue);

        var exchange = await EnsureExchangeExists(testExchange);

        var bindings = (await fixture.ManagementClient.GetQueueBindingsAsync(exchange, queue)).ToArray();

        bindings.Length.Should().Be(0);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_list_of_bindings_between_an_exchange_and_an_exchange()
    {
        var exchange1 = await EnsureExchangeExists(testExchange);
        var exchange2 = await EnsureExchangeExists(testExchange2);

        var bindings = await fixture.ManagementClient.GetExchangeBindingsAsync(exchange1, exchange2);

        bindings.Count.Should().Be(0);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_queue_by_name()
    {
        await CreateTestQueue(testQueue);
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, testQueue);
        queue.Name.Should().Be(testQueue);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_queue_by_name_with_all_detailed_information()
    {
        const int age = 10;
        const int increment = 1;
        await CreateTestQueue(testQueue);
        await Task.Delay(TimeSpan.FromSeconds(10));

        var queue = await fixture.ManagementClient.GetQueueAsync(
            Vhost,
            testQueue,
            new GetLengthsCriteria(age, increment),
            new GetRatesCriteria(age, increment)
        );

        queue.Name.Should().Be(testQueue);
        queue.MessagesDetails.Samples.Count.Should().BeGreaterThan(0);
        queue.MessagesReadyDetails.Samples.Count.Should().BeGreaterThan(0);
        queue.MessagesUnacknowledgedDetails.Samples.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_queue_by_name_with_detailed_length_information()
    {
        const int age = 10;
        const int increment = 1;
        await CreateTestQueue(testQueue);
        await Task.Delay(TimeSpan.FromSeconds(10));

        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, testQueue, new GetLengthsCriteria(age, increment));

        queue.Name.Should().Be(testQueue);
        queue.MessagesDetails.Samples.Count.Should().BeGreaterThan(0);
        queue.MessagesReadyDetails.Samples.Count.Should().BeGreaterThan(0);
        queue.MessagesUnacknowledgedDetails.Samples.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_queue_by_name_with_detailed_rates_information()
    {
        const int age = 60;
        const int increment = 10;
        await CreateTestQueue(testQueue);
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, testQueue, ratesCriteria: new GetRatesCriteria(age, increment));
        queue.Name.Should().Be(testQueue);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_queue_by_name_with_plus_char()
    {
        await CreateTestQueue(testQueueWithPlusChar);
        var queue = await fixture.ManagementClient.GetQueueAsync(Vhost, testQueueWithPlusChar);
        queue.Name.Should().Be(testQueueWithPlusChar);
    }

    [Fact]
    public async Task Should_be_able_to_get_a_user_by_name()
    {
        var userInfo = new UserInfo(testUser, "topSecret");
        await fixture.ManagementClient.CreateUserAsync(userInfo);
        (await fixture.ManagementClient.GetUserAsync(testUser)).Name.Should().Be(testUser);
    }

    [Fact]
    public async Task Should_be_able_to_get_all_the_bindings_for_a_queue()
    {
        var queue = await CreateTestQueue(testQueue);
        var bindings = await fixture.ManagementClient.GetBindingsForQueueAsync(queue);
        Assert.NotEmpty(bindings.ToList());
    }

    [Fact]
    public async Task Should_be_able_to_get_an_individual_exchange_by_name()
    {
        var vhost = new Vhost { Name = Vhost.Name };
        var exchange = await fixture.ManagementClient.GetExchangeAsync(vhost, testExchange);

        exchange.Name.Should().Be(testExchange);
    }

    [Fact]
    public async Task Should_be_able_to_get_an_individual_vhost()
    {
        await fixture.ManagementClient.CreateVhostAsync(testVHost);
        var vhost = await fixture.ManagementClient.GetVhostAsync(testVHost);
        vhost.Name.Should().Be(testVHost);
    }

    [Fact]
    public async Task Should_be_able_to_get_messages_from_a_queue()
    {
        var queue = await CreateTestQueue(testQueue);

        var defaultExchange = new Exchange { Name = "amq.default", Vhost = Vhost.Name };

        var publishInfo = new PublishInfo(
            new Dictionary<string, object>
            {
                {"app_id", "management-test"}
            },
            testQueue, "Hello World", "string");

        await fixture.ManagementClient.PublishAsync(defaultExchange, publishInfo);

        var messages = await fixture.ManagementClient.GetMessagesFromQueueAsync(queue, new GetMessagesCriteria(1, AckMode.AckRequeueFalse));
        foreach (var message in messages)
        {
            Console.Out.WriteLine("message.Payload = {0}", message.Payload);
            foreach (var property in message.Properties)
                Console.Out.WriteLine("key: '{0}', value: '{1}'", property.Key, property.Value);
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
        var exchange = await CreateExchange(testExchange);

        var publishInfo = new PublishInfo(testQueue, "Hello World");
        var result = await fixture.ManagementClient.PublishAsync(exchange, publishInfo);

        // the testExchange isn't bound to a queue
        result.Routed.Should().BeFalse();
    }

    [Fact]
    public async Task Should_check_that_the_broker_is_alive()
    {
        await fixture.ManagementClient.CreateVhostAsync(testVHost);
        var vhost = await fixture.ManagementClient.GetVhostAsync(testVHost);
        var user = (await fixture.ManagementClient.GetUsersAsync()).SingleOrDefault(x => x.Name == fixture.User);
        var permissionInfo = new PermissionInfo(user.Name);
        await fixture.ManagementClient.CreatePermissionAsync(vhost, permissionInfo);
        (await fixture.ManagementClient.IsAliveAsync(vhost)).Should().BeTrue();
    }

    [Fact]
    public async Task Should_create_a_virtual_host()
    {
        await fixture.ManagementClient.CreateVhostAsync(testVHost);
        var vhost = await fixture.ManagementClient.GetVhostAsync(testVHost);
        vhost.Name.Should().Be(testVHost);
    }

    [Fact]
    public async Task Should_create_binding()
    {
        var vhost = await fixture.ManagementClient.GetVhostAsync(Vhost.Name);
        var queue = await fixture.ManagementClient.GetQueueAsync(vhost, testQueue);
        var exchange = await fixture.ManagementClient.GetExchangeAsync(vhost, testExchange);

        var bindingInfo = new BindingInfo(testQueue);

        await fixture.ManagementClient.CreateQueueBindingAsync(exchange, queue, bindingInfo);
    }

    [Fact]
    public async Task Should_create_exchange_to_exchange_binding()
    {
        const string sourceExchangeName = "management_api_test_source_exchange";
        const string destinationExchangeName = "management_api_test_destination_exchange";

        var vhost = await fixture.ManagementClient.GetVhostAsync(Vhost.Name);
        var sourceExchangeInfo = new ExchangeInfo(sourceExchangeName, "direct");
        var destinationExchangeInfo = new ExchangeInfo(destinationExchangeName, "direct");

        await fixture.ManagementClient.CreateExchangeAsync(sourceExchangeInfo, vhost);
        var sourceExchange = await fixture.ManagementClient.GetExchangeAsync(vhost, sourceExchangeInfo.Name);

        await fixture.ManagementClient.CreateExchangeAsync(destinationExchangeInfo, vhost);
        var destinationExchange = await fixture.ManagementClient.GetExchangeAsync(vhost, destinationExchangeInfo.Name);

        await fixture.ManagementClient.CreateExchangeBindingAsync(sourceExchange, destinationExchange, new BindingInfo("#"));

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
        var vhost = await fixture.ManagementClient.GetVhostAsync(testVHost);
        await fixture.ManagementClient.DeleteVhostAsync(vhost);
    }

    [Fact]
    public async Task Should_delete_binding()
    {
        var sourceXchange = await CreateExchange("sourceXcg");
        var queue = await CreateTestQueue(testQueue);
        var bindingInfo = new BindingInfo("#");
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
        var sourceExchangeInfo = new ExchangeInfo(sourceExchangeName, "direct");
        var destinationExchangeInfo = new ExchangeInfo(destinationExchangeName, "direct");

        await fixture.ManagementClient.CreateExchangeAsync(sourceExchangeInfo, vhost);
        var sourceExchange = await fixture.ManagementClient.GetExchangeAsync(vhost, sourceExchangeInfo.Name);

        await fixture.ManagementClient.CreateExchangeAsync(destinationExchangeInfo, vhost);
        var destinationExchange = await fixture.ManagementClient.GetExchangeAsync(vhost, destinationExchangeInfo.Name);

        await fixture.ManagementClient.CreateExchangeBindingAsync(sourceExchange, destinationExchange, new BindingInfo("#"));

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
        var bindingInfo = new BindingInfo("#");
        await fixture.ManagementClient.CreateExchangeBindingAsync(sourceXchange, destinationXchange, bindingInfo);

        Assert.NotEmpty((await fixture.ManagementClient.GetBindingsWithDestinationAsync(destinationXchange)).ToList());
    }

    [Fact]
    public async Task Should_get_all_bindings_for_which_the_exchange_is_the_source()
    {
        var sourceXchange = await CreateExchange("sourceXcg");
        var destinationXchange = await CreateExchange("destinationXcg");
        var bindingInfo = new BindingInfo("#");
        await fixture.ManagementClient.CreateExchangeBindingAsync(sourceXchange, destinationXchange, bindingInfo);
        Assert.NotEmpty((await fixture.ManagementClient.GetBindingsWithSourceAsync(sourceXchange)).ToList());
    }

    [Fact]
    public async Task Should_get_bindings()
    {
        foreach (var binding in await fixture.ManagementClient.GetBindingsAsync())
        {
            Console.Out.WriteLine("binding.Destination = {0}", binding.Destination);
            Console.Out.WriteLine("binding.Source = {0}", binding.Source);
            Console.Out.WriteLine("binding.PropertiesKey = {0}", binding.PropertiesKey);
        }
    }

    [Fact(Skip = "Requires at least a consumer")]
    public async Task Should_get_consumers()
    {
        foreach (var consumer in await fixture.ManagementClient.GetConsumersAsync())
        {
            Console.Out.WriteLine("consumer.ConsumerTag = {0}", consumer.ConsumerTag);
            Console.Out.WriteLine("consumer.ChannelDetails.ConnectionName = {0}", consumer.ChannelDetails.ConnectionName);
            Console.Out.WriteLine("consumer.ChannelDetails.ConnectionName = {0}", consumer.ChannelDetails.ConnectionName);
            Console.Out.WriteLine("consumer.ChannelDetails.Node = {0}", consumer.ChannelDetails.Node);
        }
    }

    [Fact]
    public async Task Should_get_channels()
    {
        var channels = await fixture.ManagementClient.GetChannelsAsync();

        foreach (var channel in channels)
        {
            Console.Out.WriteLine("channel.Name = {0}", channel.Name);
            Console.Out.WriteLine("channel.User = {0}", channel.User);
            Console.Out.WriteLine("channel.PrefetchCount = {0}", channel.PrefetchCount);
        }
    }

    [Fact]
    public async Task Should_get_channels_per_connection()
    {
        var connections = await fixture.ManagementClient.GetConnectionsAsync();
        foreach (var connection in connections)
        {
            Console.Out.WriteLine("connection.Name = {0}", connection.Name);
            var channels = await fixture.ManagementClient.GetChannelsAsync(connection);

            foreach (var channel in channels) Console.Out.WriteLine("\tchannel.Name = {0}", channel.Name);
        }
    }

    [Fact]
    public async Task Should_get_connections()
    {
        foreach (var connection in await fixture.ManagementClient.GetConnectionsAsync())
        {
            Console.Out.WriteLine("connection.Name = {0}", connection.Name);

            var clientProperties = connection.ClientProperties;

            Console.WriteLine("User:\t{0}", clientProperties.User);
            Console.WriteLine("Application:\t{0}", clientProperties.Application);
            Console.WriteLine("ClientApi:\t{0}", clientProperties.ClientApi);
            Console.WriteLine("ApplicationLocation:\t{0}", clientProperties.ApplicationLocation);
            Console.WriteLine("Connected:\t{0}", clientProperties.Connected);
            Console.WriteLine("EasynetqVersion:\t{0}", clientProperties.EasynetqVersion);
            Console.WriteLine("MachineName:\t{0}", clientProperties.MachineName);

            //Test the dynamic nature
            Console.WriteLine("Copyright:\t{0}", ((dynamic)clientProperties).Copyright);
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

        foreach (var exchange in exchanges) Console.Out.WriteLine("exchange.Name = {0}", exchange.Name);
    }

    [Fact(Skip = "Requires at least an active federation")]
    public async Task Should_get_federations()
    {
        var federations = await fixture.ManagementClient.GetFederationsAsync();

        federations.Single().Node.Should().Be($"rabbit@{fixture.Host}");
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
        var overview = await fixture.ManagementClient.GetOverviewAsync();

        Console.Out.WriteLine("overview.ManagementVersion = {0}", overview.ManagementVersion);
        foreach (var exchangeType in overview.ExchangeTypes)
            Console.Out.WriteLine("exchangeType.Name = {0}", exchangeType.Name);
        foreach (var listener in overview.Listeners)
            Console.Out.WriteLine("listener.IpAddress = {0}", listener.IpAddress);

        Console.Out.WriteLine("overview.Messages = {0}", overview.QueueTotals?.Messages ?? 0);

        foreach (var context in overview.Contexts)
            Console.Out.WriteLine("context.Description = {0}", context.Description);
    }

    [Fact]
    public async Task Should_get_permissions()
    {
        var permissions = await fixture.ManagementClient.GetPermissionsAsync();

        foreach (var permission in permissions)
        {
            Console.Out.WriteLine("permission.User = {0}", permission.User);
            Console.Out.WriteLine("permission.Vhost = {0}", permission.Vhost);
            Console.Out.WriteLine("permission.Configure = {0}", permission.Configure);
            Console.Out.WriteLine("permission.Read = {0}", permission.Read);
            Console.Out.WriteLine("permission.Write = {0}", permission.Write);
        }
    }

    [Fact]
    public async Task Should_get_queues()
    {
        await CreateTestQueue(testQueue);
        (await fixture.ManagementClient.GetQueuesAsync()).Count.Should().BeGreaterThan(0);
    }


    [Fact]
    public async Task Should_get_queues_with_pagination()
    {
        foreach (var queue in await fixture.ManagementClient.GetQueuesAsync())
            await fixture.ManagementClient.DeleteQueueAsync(queue);

        await CreateTestQueue("1");
        await CreateTestQueue("2");

        var firstPage = await fixture.ManagementClient.GetQueuesByPageAsync(new PageCriteria(1, 1));
        firstPage
            .Should()
            .BeEquivalentTo(new PageResult<Queue>(2, 1, Array.Empty<Queue>(), 1, 2, 1, 2), x => x.Excluding(x => x.Items));

        var secondPage = await fixture.ManagementClient.GetQueuesByPageAsync(new PageCriteria(2, 1));
        secondPage
            .Should()
            .BeEquivalentTo(new PageResult<Queue>(2, 1, Array.Empty<Queue>(), 2, 2, 1, 2), x => x.Excluding(x => x.Items));
    }

    [Fact]
    public async Task Should_get_queues_by_vhost()
    {
        await fixture.ManagementClient.CreateVhostAsync(testVHost);
        var vhost = await fixture.ManagementClient.GetVhostAsync(testVHost);
        vhost.Name.Should().Be(testVHost);

        await CreateTestQueueInVhost($"{testVHost}_{testQueue}", vhost);
        (await fixture.ManagementClient.GetQueuesAsync(vhost)).Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_get_users()
    {
        var users = await fixture.ManagementClient.GetUsersAsync();

        foreach (var user in users) Console.Out.WriteLine("user.Name = {0}", user.Name);
    }

    [Fact]
    public async Task Should_get_vhosts()
    {
        var vhosts = await fixture.ManagementClient.GetVhostsAsync();

        foreach (var vhost in vhosts) Console.Out.WriteLine("vhost.Name = {0}", vhost.Name);
    }

    [Fact]
    public async Task Should_purge_a_queue()
    {
        var queue = await CreateTestQueue(testQueue);
        await fixture.ManagementClient.PurgeAsync(queue);
    }

    [Fact]
    public async Task Should_throw_when_trying_to_close_unknown_connection()
    {
        var connection = new Connection { Name = "unknown" };
        try
        {
            await fixture.ManagementClient.CloseConnectionAsync(connection);
        }
        catch (Exception e)
        {
            Assert.IsType<UnexpectedHttpStatusCodeException>(e);
        }
    }
}
