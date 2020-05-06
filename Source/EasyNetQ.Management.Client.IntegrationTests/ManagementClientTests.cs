using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ.Management.Client.Model;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.Management.Client.IntegrationTests
{
    [Collection("Rabbitmq collection")]
    public class ManagementClientTests
    {
        public ManagementClientTests(RabbitMqFixture fixture)
        {
            hostUrl = $"http://{fixture.RabbitHostForManagement}";
            managementClient = new ManagementClient(hostUrl, username, password, port);
        }

        private const string testExchange = "management_api_test_exchange";
        private const string testExchange2 = "management_api_test_exchange2";
        private const string testExchangetestQueueWithPlusChar = "management_api_test_exchange+plus+test";
        private const string testQueue = "management_api_test_queue";
        private const string testQueueWithPlusChar = "management_api_test_queue+plus+test";
        private const string testUser = "mikey";

        private readonly string hostUrl;
        private readonly string username = Configuration.RabbitMqUser;
        private readonly string password = Configuration.RabbitMqPassword;
        private readonly int port = Configuration.RabbitMqManagementPort;
        private readonly string vhostName = Configuration.RabbitMqVirtualHostName;
        private readonly IManagementClient managementClient;
        private readonly string rabbitHostName = Configuration.RabbitMqHostName;

        private Task<Exchange> CreateExchange(string exchangeName)
        {
            return managementClient.CreateExchangeAsync(new ExchangeInfo(exchangeName, "direct"),
                Configuration.RabbitMqVirtualHost);
        }

        private async Task<Queue> EnsureQueueExists(string managementApiTestQueue)
        {
            return (await managementClient.GetQueuesAsync().ConfigureAwait(false))
                   .SingleOrDefault(x => x.Name == managementApiTestQueue)
                   ?? await CreateTestQueue(managementApiTestQueue).ConfigureAwait(false);
        }

        private async Task<Queue> CreateTestQueue(string queueName)
        {
            var vhost = await managementClient.GetVhostAsync(vhostName).ConfigureAwait(false);
            return await CreateTestQueueInVhost(queueName, vhost);
        }

        private async Task<Queue> CreateTestQueueInVhost(string queueName, Vhost vhost)
        {
            var queueInfo = new QueueInfo(queueName);
            await managementClient.CreateQueueAsync(queueInfo, vhost).ConfigureAwait(false);
            return await managementClient.GetQueueAsync(queueName, vhost).ConfigureAwait(false);
        }

        private async Task<Exchange> EnsureExchangeExists(string exchangeName)
        {
            return (await managementClient
                       .GetExchangesAsync().ConfigureAwait(false))
                   .SingleOrDefault(x => x.Name == exchangeName)
                   ?? await CreateExchange(exchangeName).ConfigureAwait(false);
        }

        private const string testVHost = "management_test_virtual_host";

        [Fact]
        public async Task Should_be_able_to_change_the_password_of_a_user()
        {
            var userInfo = new UserInfo(testUser, "topSecret").AddTag("monitoring").AddTag("management");
            var user = await managementClient.CreateUserAsync(userInfo).ConfigureAwait(false);

            var updatedUser = await managementClient.ChangeUserPasswordAsync(testUser, "newPassword")
                .ConfigureAwait(false);

            updatedUser.Name.Should().Be(user.Name);
            updatedUser.Tags.Should().Be(user.Tags);
            updatedUser.PasswordHash.Should().NotBe(user.PasswordHash);
        }

        [Fact(Skip = "Requires at least a connection")]
        public async Task Should_be_able_to_close_connection()
        {
            // first get a connection
            var connections = await managementClient.GetConnectionsAsync().ConfigureAwait(false);

            // then close it
            await managementClient.CloseConnectionAsync(connections.First()).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_be_able_to_configure_request()
        {
            var client = new ManagementClient(hostUrl, username, password, port, configureRequest:
                req => req.Headers.Add("x-not-used", "some_value"));

            await client.GetOverviewAsync().ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_be_able_to_create_a_queue()
        {
            (await managementClient.CreateQueueAsync(new QueueInfo(testQueue), Configuration.RabbitMqVirtualHost)
                    .ConfigureAwait(false))
                .Name.Should().Be(testQueue);
        }

        [Fact]
        public async Task Should_be_able_to_create_a_queue_with_arguments()
        {
            const string exchangeName = "test-dead-letter-exchange";
            const string argumentKey = "x-dead-letter-exchange";
            var queueInfo = new QueueInfo($"{testQueue}1");
            queueInfo.Arguments.Add(argumentKey, exchangeName);
            var queue = await managementClient.CreateQueueAsync(queueInfo, Configuration.RabbitMqVirtualHost)
                .ConfigureAwait(false);
            queue.Arguments[argumentKey].Should().NotBeNull();
            queue.Arguments[argumentKey].Should().Be(exchangeName);
        }

        [Fact]
        public async Task Should_be_able_to_create_a_queue_with_plus_char_in_the_name()
        {
            var queueInfo = new QueueInfo(testQueueWithPlusChar);
            var queue = await managementClient.CreateQueueAsync(queueInfo, Configuration.RabbitMqVirtualHost)
                .ConfigureAwait(false);
            queue.Name.Should().Be(testQueueWithPlusChar);
        }

        [Fact]
        public async Task Should_be_able_to_create_a_user()
        {
            var userInfo = new UserInfo(testUser, "topSecret").AddTag("administrator");

            var user = await managementClient.CreateUserAsync(userInfo).ConfigureAwait(false);
            user.Name.Should().Be(testUser);
        }

        [Fact]
        public async Task Should_be_able_to_create_a_user_with_password_hash()
        {
            var testUser = "hash_user";
            // Hash calculated using RabbitMq hash computing algorithm using Sha256
            // See https://www.rabbitmq.com/passwords.html
            var passwordHash = "Qlp9Dgrqvx1S1VkuYsoWwgUD2XW2gZLuqQwreE+PAsPZETgo"; //"topSecret"
            var userInfo = new UserInfo(testUser, passwordHash, true).AddTag("administrator");

            var user = await managementClient.CreateUserAsync(userInfo).ConfigureAwait(false);
            user.Name.Should().Be(testUser);
        }

        [Fact]
        public async Task Should_be_able_to_create_a_user_with_the_policymaker_tag()
        {
            var userInfo = new UserInfo(testUser, "topSecret").AddTag("policymaker");

            var user = await managementClient.CreateUserAsync(userInfo).ConfigureAwait(false);
            user.Name.Should().Be(testUser);
        }

        [Fact]
        public async Task Should_be_able_to_create_a_user_without_password()
        {
            var testUser = "empty";
            var userInfo = new UserInfo(testUser, "", true).AddTag("administrator");

            var user = await managementClient.CreateUserAsync(userInfo).ConfigureAwait(false);
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
            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
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
            }).ConfigureAwait(false);
            Assert.Equal(1, (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Priority == priority
                     && p.Definition.HaMode == haMode
                     && p.Definition.HaSyncMode == haSyncMode
                     && p.Definition.AlternateExchange == alternateExchange
                     && p.Definition.DeadLetterExchange == deadLetterExchange
                     && p.Definition.DeadLetterRoutingKey == deadLetterRoutingKey
                     && p.Definition.MessageTtl == messageTtl
                     && p.Definition.Expires == expires
                     && p.Definition.MaxLength == maxLength));
        }

        [Fact]
        public async Task Should_be_able_to_create_alternate_exchange_policy()
        {
            const string policyName = "a-sample-alternate-exchange-policy";
            const string alternateExchange = "a-sample-alternate-exchange";
            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    AlternateExchange = alternateExchange
                }
            }).ConfigureAwait(false);
            Assert.Equal(1, (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.AlternateExchange == alternateExchange));
        }

        [Fact]
        public async Task Should_be_able_to_create_an_exchange()
        {
            var exchange = await CreateExchange(testExchange).ConfigureAwait(false);
            exchange.Name.Should().Be(testExchange);
        }

        [Fact]
        public async Task Should_be_able_to_create_an_exchange_with_plus_char_in_the_name()
        {
            var exhangeInfo = new ExchangeInfo(testExchangetestQueueWithPlusChar, "direct");
            var queue = await managementClient.CreateExchangeAsync(exhangeInfo, Configuration.RabbitMqVirtualHost)
                .ConfigureAwait(false);
            queue.Name.Should().Be(testExchangetestQueueWithPlusChar);
        }

        [Fact]
        public async Task Should_be_able_to_create_dead_letter_exchange_policy()
        {
            const string policyName = "a-sample-dead-letter-exchange";
            const string deadLetterExchange = "a-sample-dead-letter-exchange";
            const string deadLetterRoutingKey = "a-sample-dead-letter-exchange-key";
            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    DeadLetterExchange = deadLetterExchange,
                    DeadLetterRoutingKey = deadLetterRoutingKey
                }
            }).ConfigureAwait(false);
            Assert.Equal(1, (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.DeadLetterExchange == deadLetterExchange
                     && p.Definition.DeadLetterRoutingKey == deadLetterRoutingKey));
        }

        [Fact]
        public async Task Should_be_able_to_create_exchanges_only_policies()
        {
            const string policyName = "asamplepolicy-exchange-only";
            const HaMode haMode = HaMode.All;
            const HaSyncMode haSyncMode = HaSyncMode.Automatic;
            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                ApplyTo = ApplyMode.Exchanges,
                Definition = new PolicyDefinition
                {
                    HaMode = haMode,
                    HaSyncMode = haSyncMode
                }
            }).ConfigureAwait(false);
            Assert.Equal(1, (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.ApplyTo == ApplyMode.Exchanges
                     && p.Definition.HaMode == haMode
                     && p.Definition.HaSyncMode == haSyncMode));
        }

        [Fact]
        public async Task Should_be_able_to_create_expires_policy()
        {
            const string policyName = "a-sample-expires";
            const uint expires = 10000;
            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    Expires = expires
                }
            }).ConfigureAwait(false);
            Assert.Equal(1, (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.Expires == expires));
        }

        [Fact]
        public async Task Should_be_able_to_create_federation_upstream_policy()
        {
            const string policyName = "a-sample-federation-upstream-policy";

            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    FederationUpstream = "my-upstream"
                }
            }).ConfigureAwait(false);
            Assert.Equal(1, (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.FederationUpstream == "my-upstream"));
        }

        [Fact]
        public async Task Should_be_able_to_create_federation_upstream_set_policy()
        {
            const string policyName = "a-sample-federation-upstream-set-policy";

            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    FederationUpstreamSet = "my-upstream-set"
                }
            }).ConfigureAwait(false);
            Assert.Equal(1, (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.FederationUpstreamSet == "my-upstream-set"));
        }

        [Fact]
        public async Task Should_be_able_to_create_max_length_policy()
        {
            const string policyName = "a-sample-max-length";
            const uint maxLength = 500;
            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    MaxLength = maxLength
                }
            }).ConfigureAwait(false);
            Assert.Equal(1, (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.MaxLength == maxLength));
        }

        [Fact]
        public async Task Should_be_able_to_create_message_ttl_policy()
        {
            const string policyName = "a-sample-message-ttl";
            const uint messageTtl = 5000;
            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    MessageTtl = messageTtl
                }
            }).ConfigureAwait(false);
            Assert.Equal(1, (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.MessageTtl == messageTtl));
        }

        [Fact]
        public async Task Should_be_able_to_create_parameter()
        {
            await managementClient.CreateParameterAsync(new Parameter
            {
                Component = "federation-upstream",
                Name = "myfakefederationupstream1",
                Vhost = vhostName,
                Value = new {value = new {uri = $"amqp://{username}:{password}@{rabbitHostName}"}}
            }).ConfigureAwait(false);
            Assert.Contains(await managementClient.GetParametersAsync().ConfigureAwait(false),
                p => p.Name == "myfakefederationupstream1");
        }

        [Fact]
        public async Task Should_be_able_to_create_permissions()
        {
            var user = (await managementClient.GetUsersAsync().ConfigureAwait(false)).SingleOrDefault(x =>
                x.Name == testUser);
            if (user == null)
                throw new EasyNetQTestException(string.Format("user '{0}' hasn't been created", testUser));
            var vhost =
                (await managementClient.GetVhostsAsync().ConfigureAwait(false)).SingleOrDefault(
                    x => x.Name == testVHost);
            if (vhost == null)
                throw new EasyNetQTestException(string.Format("Test vhost: '{0}' has not been created", testVHost));

            var permissionInfo = new PermissionInfo(user, vhost);
            await managementClient.CreatePermissionAsync(permissionInfo).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_be_able_to_create_permissions_in_default_Vhost()
        {
            var user = (await managementClient.GetUsersAsync().ConfigureAwait(false)).SingleOrDefault(x =>
                x.Name == testUser);
            if (user == null)
            {
                //create user if it does not exists
                var userInfo = new UserInfo(testUser, "topSecret").AddTag("administrator");
                user = await managementClient.CreateUserAsync(userInfo).ConfigureAwait(false);
            }

            var vhost =
                (await managementClient.GetVhostsAsync().ConfigureAwait(false)).SingleOrDefault(
                    x => x.Name == vhostName);
            if (vhost == null)
                throw new EasyNetQTestException(string.Format("Default vhost: '{0}' has not been created", testVHost));

            var permissionInfo = new PermissionInfo(user, vhost);
            await managementClient.CreatePermissionAsync(permissionInfo).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_be_able_to_create_policies()
        {
            const string policyName = "asamplepolicy";
            const HaMode haMode = HaMode.All;
            const HaSyncMode haSyncMode = HaSyncMode.Automatic;
            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    HaMode = haMode,
                    HaSyncMode = haSyncMode
                }
            }).ConfigureAwait(false);
            Assert.Equal(1, (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.ApplyTo == ApplyMode.All
                     && p.Definition.HaMode == haMode
                     && p.Definition.HaSyncMode == haSyncMode));
        }

        [Fact]
        public async Task Should_be_able_to_create_queues_only_policies()
        {
            const string policyName = "asamplepolicy-queue-only";
            const HaMode haMode = HaMode.All;
            const HaSyncMode haSyncMode = HaSyncMode.Automatic;
            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                ApplyTo = ApplyMode.Queues,
                Definition = new PolicyDefinition
                {
                    HaMode = haMode,
                    HaSyncMode = haSyncMode
                }
            }).ConfigureAwait(false);
            Assert.Equal(1, (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.ApplyTo == ApplyMode.Queues
                     && p.Definition.HaMode == haMode
                     && p.Definition.HaSyncMode == haSyncMode));
        }

        [Fact]
        public async Task Should_be_able_to_delete_a_queue()
        {
            var queue = await CreateTestQueue(testQueue).ConfigureAwait(false);
            await managementClient.DeleteQueueAsync(queue).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_be_able_to_delete_a_user()
        {
            var user = await managementClient.GetUserAsync(testUser).ConfigureAwait(false);
            await managementClient.DeleteUserAsync(user).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_be_able_to_delete_an_exchange()
        {
            const string exchangeName = "delete-xcg";
            var exchange = await CreateExchange(exchangeName).ConfigureAwait(false);

            await managementClient.DeleteExchangeAsync(exchange).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_be_able_to_delete_an_exchange_with_pluses()
        {
            var exchange = await CreateExchange(testExchangetestQueueWithPlusChar).ConfigureAwait(false);
            await managementClient.DeleteExchangeAsync(exchange).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_be_able_to_delete_permissions()
        {
            var userInfo = new UserInfo(testUser, "topSecret").AddTag("monitoring").AddTag("management");
            var user = await managementClient.CreateUserAsync(userInfo).ConfigureAwait(false);
            var vhost = await managementClient.CreateVhostAsync(testVHost).ConfigureAwait(false);
            var permissionInfo = new PermissionInfo(user, vhost);
            await managementClient.CreatePermissionAsync(permissionInfo).ConfigureAwait(false);

            var permission = (await managementClient.GetPermissionsAsync().ConfigureAwait(false))
                .SingleOrDefault(x => x.User == testUser && x.Vhost == testVHost);

            if (permission == null)
                throw new EasyNetQTestException(string.Format("No permission for vhost: {0} and user: {1}",
                    testVHost, testUser));

            await managementClient.DeletePermissionAsync(permission).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_be_able_to_delete_policies()
        {
            const string policyName = "asamplepolicy";
            await managementClient.CreatePolicyAsync(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    HaMode = HaMode.All,
                    HaSyncMode = HaSyncMode.Automatic
                }
            }).ConfigureAwait(false);
            Assert.Equal(1,
                (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(p =>
                    p.Name == policyName && p.Vhost == vhostName));
            await managementClient.DeletePolicyAsync(policyName, new Vhost {Name = vhostName}).ConfigureAwait(false);
            Assert.Equal(0,
                (await managementClient.GetPoliciesAsync().ConfigureAwait(false)).Count(p =>
                    p.Name == policyName && p.Vhost == vhostName));
        }

        [Fact]
        public async Task Should_be_able_to_get_a_list_of_bindings_between_an_exchange_and_a_queue()
        {
            var queue = await EnsureQueueExists(testQueue).ConfigureAwait(false);

            var exchange = await EnsureExchangeExists(testExchange).ConfigureAwait(false);

            var bindings = (await managementClient.GetBindingsAsync(exchange, queue).ConfigureAwait(false)).ToArray();

            bindings.Length.Should().NotBe(0);

            foreach (var binding in bindings)
            {
                Console.Out.WriteLine("binding.RoutingKey = {0}", binding.RoutingKey);
                Console.Out.WriteLine("binding.PropertiesKey = {0}", binding.PropertiesKey);
            }
        }

        [Fact]
        public async Task Should_be_able_to_get_a_list_of_bindings_between_an_exchange_and_an_exchange()
        {
            var exchange1 = await EnsureExchangeExists(testExchange).ConfigureAwait(false);
            var exchange2 = await EnsureExchangeExists(testExchange2).ConfigureAwait(false);

            var bindings =
                (await managementClient.GetBindingsAsync(exchange1, exchange2).ConfigureAwait(false)).ToArray();

            bindings.Length.Should().NotBe(0);

            foreach (var binding in bindings)
            {
                Console.Out.WriteLine("binding.RoutingKey = {0}", binding.RoutingKey);
                Console.Out.WriteLine("binding.PropertiesKey = {0}", binding.PropertiesKey);
            }
        }

        [Fact]
        public async Task Should_be_able_to_get_a_queue_by_name()
        {
            await CreateTestQueue(testQueue).ConfigureAwait(false);
            var queue = await managementClient.GetQueueAsync(testQueue, Configuration.RabbitMqVirtualHost)
                .ConfigureAwait(false);
            queue.Name.Should().Be(testQueue);
        }

        [Fact]
        public async Task Should_be_able_to_get_a_queue_by_name_with_all_detailed_information()
        {
            const int age = 10;
            const int increment = 1;
            await CreateTestQueue(testQueue).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);

            var queue = await managementClient.GetQueueAsync(testQueue, Configuration.RabbitMqVirtualHost,
                new GetLengthsCriteria(age, increment), new GetRatesCriteria(age, increment)).ConfigureAwait(false);

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
            await CreateTestQueue(testQueue).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);

            var queue = await managementClient
                .GetQueueAsync(testQueue, Configuration.RabbitMqVirtualHost, new GetLengthsCriteria(age, increment))
                .ConfigureAwait(false);

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
            await CreateTestQueue(testQueue).ConfigureAwait(false);
            var queue = await managementClient.GetQueueAsync(testQueue, Configuration.RabbitMqVirtualHost,
                ratesCriteria: new GetRatesCriteria(age, increment)).ConfigureAwait(false);
            queue.Name.Should().Be(testQueue);
        }

        [Fact]
        public async Task Should_be_able_to_get_a_queue_by_name_with_plus_char()
        {
            await CreateTestQueue(testQueueWithPlusChar).ConfigureAwait(false);
            var queue = await managementClient.GetQueueAsync(testQueueWithPlusChar, Configuration.RabbitMqVirtualHost)
                .ConfigureAwait(false);
            queue.Name.Should().Be(testQueueWithPlusChar);
        }

        [Fact]
        public async Task Should_be_able_to_get_a_user_by_name()
        {
            var userInfo = new UserInfo(testUser, "topSecret");
            await managementClient.CreateUserAsync(userInfo).ConfigureAwait(false);

            (await managementClient.GetUserAsync(testUser).ConfigureAwait(false)).Name.Should().Be(testUser);
        }

        [Fact]
        public async Task Should_be_able_to_get_all_the_bindings_for_a_queue()
        {
            var queue = await CreateTestQueue(testQueue).ConfigureAwait(false);
            var bindings = await managementClient.GetBindingsForQueueAsync(queue).ConfigureAwait(false);
            Assert.NotEmpty(bindings.ToList());
        }

        [Fact]
        public async Task Should_be_able_to_get_an_individual_exchange_by_name()
        {
            var vhost = new Vhost {Name = vhostName};
            var exchange = await managementClient.GetExchangeAsync(testExchange, vhost).ConfigureAwait(false);

            exchange.Name.Should().Be(testExchange);
        }

        [Fact]
        public async Task Should_be_able_to_get_an_individual_vhost()
        {
            await managementClient.CreateVhostAsync(testVHost).ConfigureAwait(false);
            var vhost = await managementClient.GetVhostAsync(testVHost).ConfigureAwait(false);
            vhost.Name.Should().Be(testVHost);
        }

        [Fact]
        public async Task Should_be_able_to_get_messages_from_a_queue()
        {
            var queue = await CreateTestQueue(testQueue).ConfigureAwait(false);

            var defaultExchange = new Exchange {Name = "amq.default", Vhost = vhostName};

            var publishInfo = new PublishInfo(
                new Dictionary<string, object>
                {
                    {"app_id", "management-test"}
                },
                testQueue, "Hello World", "string");

            await managementClient.PublishAsync(defaultExchange, publishInfo).ConfigureAwait(false);

            foreach (var message in await managementClient
                .GetMessagesFromQueueAsync(queue, new GetMessagesCriteria(1, Ackmodes.ack_requeue_false))
                .ConfigureAwait(false))
            {
                Console.Out.WriteLine("message.Payload = {0}", message.Payload);
                foreach (var property in message.Properties)
                    Console.Out.WriteLine("key: '{0}', value: '{1}'", property.Key, property.Value);
            }
        }

        [Fact]
        public void Should_be_able_to_get_policies_list()
        {
            var policies = managementClient.GetPoliciesAsync();
            Assert.NotNull(policies);
        }

        [Fact]
        public async Task Should_be_able_to_list_parameters()
        {
            var parameters = await managementClient.GetParametersAsync().ConfigureAwait(false);
            Assert.NotNull(parameters);
        }

        [Fact]
        public async Task Should_be_able_to_publish_to_an_exchange()
        {
            var exchange = await CreateExchange(testExchange).ConfigureAwait(false);

            var publishInfo = new PublishInfo(testQueue, "Hello World");
            var result = await managementClient.PublishAsync(exchange, publishInfo).ConfigureAwait(false);

            // the testExchange isn't bound to a queue
            result.Routed.Should().BeFalse();
        }

        [Fact]
        public async Task Should_check_that_the_broker_is_alive()
        {
            var vhost = await managementClient.CreateVhostAsync(testVHost).ConfigureAwait(false);
            var user = (await managementClient.GetUsersAsync().ConfigureAwait(false)).SingleOrDefault(x =>
                x.Name == username);
            var permissionInfo = new PermissionInfo(user, vhost);
            await managementClient.CreatePermissionAsync(permissionInfo).ConfigureAwait(false);

            (await managementClient.IsAliveAsync(vhost).ConfigureAwait(false)).Should().BeTrue();
        }

        [Fact]
        public async Task Should_create_a_virtual_host()
        {
            var vhost = await managementClient.CreateVhostAsync(testVHost).ConfigureAwait(false);
            vhost.Name.Should().Be(testVHost);
        }

        [Fact]
        public async Task Should_create_binding()
        {
            var vhost = await managementClient.GetVhostAsync(vhostName).ConfigureAwait(false);
            var queue = await managementClient.GetQueueAsync(testQueue, vhost).ConfigureAwait(false);
            var exchange = await managementClient.GetExchangeAsync(testExchange, vhost).ConfigureAwait(false);

            var bindingInfo = new BindingInfo(testQueue);

            await managementClient.CreateBindingAsync(exchange, queue, bindingInfo).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_create_exchange_to_exchange_binding()
        {
            const string sourceExchangeName = "management_api_test_source_exchange";
            const string destinationExchangeName = "management_api_test_destination_exchange";

            var vhost = await managementClient.GetVhostAsync(vhostName).ConfigureAwait(false);
            var sourceExchangeInfo = new ExchangeInfo(sourceExchangeName, "direct");
            var destinationExchangeInfo = new ExchangeInfo(destinationExchangeName, "direct");

            var sourceExchange =
                await managementClient.CreateExchangeAsync(sourceExchangeInfo, vhost).ConfigureAwait(false);
            var destinationExchange = await managementClient.CreateExchangeAsync(destinationExchangeInfo, vhost)
                .ConfigureAwait(false);

            await managementClient.CreateBindingAsync(sourceExchange, destinationExchange, new BindingInfo("#"))
                .ConfigureAwait(false);

            var binding = (await managementClient.GetBindingsWithSourceAsync(sourceExchange).ConfigureAwait(false))
                .First();

            await managementClient.DeleteExchangeAsync(sourceExchange).ConfigureAwait(false);
            await managementClient.DeleteExchangeAsync(destinationExchange).ConfigureAwait(false);

            Assert.Equal("exchange", binding.DestinationType);
            Assert.Equal(destinationExchangeName, binding.Destination);
            Assert.Equal("#", binding.RoutingKey);
        }

        [Fact]
        public async Task Should_delete_a_virtual_host()
        {
            var vhost = await managementClient.GetVhostAsync(testVHost).ConfigureAwait(false);
            await managementClient.DeleteVhostAsync(vhost).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_delete_binding()
        {
            var sourceXchange = await CreateExchange("sourceXcg").ConfigureAwait(false);
            var queue = await CreateTestQueue(testQueue).ConfigureAwait(false);
            var bindingInfo = new BindingInfo("#");
            await managementClient.CreateBindingAsync(sourceXchange, queue, bindingInfo).ConfigureAwait(false);
            var binding = (await managementClient.GetBindingsAsync(sourceXchange, queue).ConfigureAwait(false)).First();
            await managementClient.DeleteBindingAsync(binding).ConfigureAwait(false);
            await managementClient.DeleteExchangeAsync(sourceXchange).ConfigureAwait(false);
            await managementClient.DeleteQueueAsync(queue).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_delete_exchange_to_exchange_binding()
        {
            const string sourceExchangeName = "management_api_test_source_exchange";
            const string destinationExchangeName = "management_api_test_destination_exchange";

            var vhost = await managementClient.GetVhostAsync(vhostName).ConfigureAwait(false);
            var sourceExchangeInfo = new ExchangeInfo(sourceExchangeName, "direct");
            var destinationExchangeInfo = new ExchangeInfo(destinationExchangeName, "direct");

            var sourceExchange =
                await managementClient.CreateExchangeAsync(sourceExchangeInfo, vhost).ConfigureAwait(false);
            var destinationExchange = await managementClient.CreateExchangeAsync(destinationExchangeInfo, vhost)
                .ConfigureAwait(false);

            await managementClient.CreateBindingAsync(sourceExchange, destinationExchange, new BindingInfo("#"))
                .ConfigureAwait(false);

            var binding = (await managementClient.GetBindingsAsync(sourceExchange, destinationExchange)
                .ConfigureAwait(false)).First();

            await managementClient.DeleteBindingAsync(binding).ConfigureAwait(false);
            await managementClient.DeleteExchangeAsync(sourceExchange).ConfigureAwait(false);
            await managementClient.DeleteExchangeAsync(destinationExchange).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_disable_tracing()
        {
            var vhost = await managementClient.GetVhostAsync(vhostName).ConfigureAwait(false);
            await managementClient.DisableTracingAsync(vhost).ConfigureAwait(false);
            var vhostAfterUpdate = await managementClient.GetVhostAsync(vhostName).ConfigureAwait(false);
            Assert.False(vhostAfterUpdate.Tracing);
        }

        [Fact]
        public async Task Should_enable_tracing()
        {
            var vhost = await managementClient.GetVhostAsync(vhostName).ConfigureAwait(false);
            await managementClient.EnableTracingAsync(vhost).ConfigureAwait(false);
            var vhostAfterUpdate = await managementClient.GetVhostAsync(vhostName).ConfigureAwait(false);
            Assert.True(vhostAfterUpdate.Tracing);
        }

        [Fact]
        public async Task Should_get_all_bindings_for_which_the_exchange_is_the_destination()
        {
            var sourceXchange = await CreateExchange("sourceXcg").ConfigureAwait(false);
            var destionationXchange = await CreateExchange("destinationXcg").ConfigureAwait(false);
            var bindingInfo = new BindingInfo("#");
            await managementClient.CreateBindingAsync(sourceXchange, destionationXchange, bindingInfo)
                .ConfigureAwait(false);

            Assert.NotEmpty((await managementClient.GetBindingsWithDestinationAsync(destionationXchange)
                .ConfigureAwait(false)).ToList());
        }

        [Fact]
        public async Task Should_get_all_bindings_for_which_the_exchange_is_the_source()
        {
            var sourceXchange = await CreateExchange("sourceXcg").ConfigureAwait(false);
            var destionationXchange = await CreateExchange("destinationXcg").ConfigureAwait(false);
            var bindingInfo = new BindingInfo("#");
            await managementClient.CreateBindingAsync(sourceXchange, destionationXchange, bindingInfo)
                .ConfigureAwait(false);
            Assert.NotEmpty((await managementClient.GetBindingsWithSourceAsync(sourceXchange).ConfigureAwait(false))
                .ToList());
        }

        [Fact]
        public async Task Should_get_bindings()
        {
            foreach (var binding in await managementClient.GetBindingsAsync().ConfigureAwait(false))
            {
                Console.Out.WriteLine("binding.Destination = {0}", binding.Destination);
                Console.Out.WriteLine("binding.Source = {0}", binding.Source);
                Console.Out.WriteLine("binding.PropertiesKey = {0}", binding.PropertiesKey);
            }
        }

        [Fact]
        public async Task Should_get_channels()
        {
            var channels = await managementClient.GetChannelsAsync().ConfigureAwait(false);

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
            var connections = await managementClient.GetConnectionsAsync().ConfigureAwait(false);
            foreach (var connection in connections)
            {
                Console.Out.WriteLine("connection.Name = {0}", connection.Name);
                var channels = await managementClient.GetChannelsAsync(connection).ConfigureAwait(false);

                foreach (var channel in channels) Console.Out.WriteLine("\tchannel.Name = {0}", channel.Name);
            }
        }

        [Fact]
        public async Task Should_get_connections()
        {
            foreach (var connection in await managementClient.GetConnectionsAsync().ConfigureAwait(false))
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
                Console.WriteLine("Copyright:\t{0}", ((dynamic) clientProperties).Copyright);
            }
        }

        [Fact]
        public async Task Should_get_definitions()
        {
            var definitions = await managementClient.GetDefinitionsAsync().ConfigureAwait(false);

            definitions.RabbitVersion[0].Should().Be('3');
        }

        [Fact]
        public async Task Should_get_exchanges()
        {
            var exchanges = await managementClient.GetExchangesAsync().ConfigureAwait(false);

            foreach (var exchange in exchanges) Console.Out.WriteLine("exchange.Name = {0}", exchange.Name);
        }

        [Fact(Skip = "Requires at least an active federation")]
        public async Task Should_get_federations()
        {
            var federations = await managementClient.GetFederationAsync().ConfigureAwait(false);

            federations.Single().Node.Should().Be($"rabbit@{rabbitHostName}");
        }

        [Fact]
        public async Task Should_get_nodes()
        {
            var nodes = (await managementClient.GetNodesAsync().ConfigureAwait(false)).ToList();

            nodes.Count.Should().NotBe(0);
            nodes[0].Name.Should().Be($"rabbit@{rabbitHostName}");
        }

        [Fact]
        public async Task Should_get_overview()
        {
            var overview = await managementClient.GetOverviewAsync().ConfigureAwait(false);

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
            var permissions = await managementClient.GetPermissionsAsync().ConfigureAwait(false);

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
            await CreateTestQueue(testQueue).ConfigureAwait(false);
            (await managementClient.GetQueuesAsync().ConfigureAwait(false)).ToList().Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Should_get_queues_by_vhost()
        {
            var vhost = await managementClient.CreateVhostAsync(testVHost).ConfigureAwait(false);
            vhost.Name.Should().Be(testVHost);

            var queueName = $"{testVHost}_{testQueue}";

            await CreateTestQueueInVhost(queueName, vhost).ConfigureAwait(false);
            (await managementClient.GetQueuesAsync(vhost).ConfigureAwait(false)).ToList().Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Should_get_users()
        {
            var users = await managementClient.GetUsersAsync().ConfigureAwait(false);

            foreach (var user in users) Console.Out.WriteLine("user.Name = {0}", user.Name);
        }

        [Fact]
        public async Task Should_get_vhosts()
        {
            var vhosts = await managementClient.GetVhostsAsync().ConfigureAwait(false);

            foreach (var vhost in vhosts) Console.Out.WriteLine("vhost.Name = {0}", vhost.Name);
        }

        [Fact]
        public async Task Should_purge_a_queue()
        {
            var queue = await CreateTestQueue(testQueue).ConfigureAwait(false);
            await managementClient.PurgeAsync(queue).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_throw_when_trying_to_close_unknown_connection()
        {
            var connection = new Connection {Name = "unknown"};
            try
            {
                await managementClient.CloseConnectionAsync(connection).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Assert.IsType<UnexpectedHttpStatusCodeException>(e);
            }
        }
    }
}
