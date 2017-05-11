﻿// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;
using EasyNetQ.Management.Client.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EasyNetQ.Management.Client.IntegrationTests
{
    [Integration]
    [Explicit("requires a rabbitMQ instance on localhost to run")]
    public class ManagementClientTests
    {
        private readonly IManagementClient managementClient;

        private const string hostUrl = "http://localhost";
        private const string username = "guest";
        private const string password = "guest";
        private const string vhostName = "/";
        private const string testExchange = "management_api_test_exchange";
        private const string testExchange2 = "management_api_test_exchange2";
        private const string testExchangetestQueueWithPlusChar = "management_api_test_exchange+plus+test";
        private const string testQueue = "management_api_test_queue";
        private const string testQueueWithPlusChar = "management_api_test_queue+plus+test";
        private const string testUser = "mikey";

        public ManagementClientTests()
        {
            managementClient = new ManagementClient(hostUrl, username, password);
        }

        [Fact]
        public void Should_be_able_to_configure_request()
        {
            var client = new ManagementClient(hostUrl, username, password, configureRequest: 
                req => req.Headers.Add("x-not-used", "some_value"));

            client.GetOverview();
        }

        [Fact]
        public void Should_get_overview()
        {
            var overview = managementClient.GetOverview();

            Console.Out.WriteLine("overview.ManagementVersion = {0}", overview.ManagementVersion);
            foreach (var exchangeType in overview.ExchangeTypes)
            {
                Console.Out.WriteLine("exchangeType.Name = {0}", exchangeType.Name);
            }
            foreach (var listener in overview.Listeners)
            {
                Console.Out.WriteLine("listener.IpAddress = {0}", listener.IpAddress);
            }

            Console.Out.WriteLine("overview.Messages = {0}", overview.QueueTotals != null ? overview.QueueTotals.Messages : 0);

            foreach (var context in overview.Contexts)
            {
                Console.Out.WriteLine("context.Description = {0}", context.Description);
            }
        }

        [Fact]
        public void Should_get_nodes()
        {
            var nodes = managementClient.GetNodes();

            nodes.Count().ShouldNotEqual(0);
            nodes.First().Name.ShouldEqual("rabbit@" + Environment.MachineName);
        }

        [Fact]
        public void Should_get_definitions()
        {
            var definitions = managementClient.GetDefinitions();

            definitions.RabbitVersion[0].ShouldEqual('3');
        }

        [Fact]
        public void Should_get_connections()
        {
            var connections = managementClient.GetConnections();

            foreach (var connection in connections)
            {
                Console.Out.WriteLine("connection.Name = {0}", connection.Name);

                ClientProperties clientProperties = connection.ClientProperties;

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
        public void Should_be_able_to_close_connection()
        {
            // first get a connection
            var connections = managementClient.GetConnections();

            // then close it
            managementClient.CloseConnection(connections.First());
        }

        [Fact]
        public void Should_throw_when_trying_to_close_unknown_connection()
        {
            var connection = new Connection {Name = "unknown"};
            Assert.Throws<UnexpectedHttpStatusCodeException>(() => managementClient.CloseConnection(connection));
        }

        [Fact]
        public void Should_get_channels()
        {
            var channels = managementClient.GetChannels();

            foreach (var channel in channels)
            {
                Console.Out.WriteLine("channel.Name = {0}", channel.Name);
                Console.Out.WriteLine("channel.User = {0}", channel.User);
                Console.Out.WriteLine("channel.PrefetchCount = {0}", channel.PrefetchCount);
            }
        }

        [Fact]
        public void Should_get_exchanges()
        {
            var exchanges = managementClient.GetExchanges();

            foreach (Exchange exchange in exchanges)
            {
                Console.Out.WriteLine("exchange.Name = {0}", exchange.Name);
            }
        }

        [Fact]
        public void Should_be_able_to_get_an_individual_exchange_by_name()
        {
            var vhost = new Vhost { Name = vhostName };
            var exchange = managementClient.GetExchange(testExchange, vhost);

            exchange.Name.ShouldEqual(testExchange);
        }

        [Fact]
        public void Should_be_able_to_create_an_exchange()
        {
            var exchange = CreateExchange(testExchange);
            exchange.Name.ShouldEqual(testExchange);
        }

        private Exchange CreateExchange(string exchangeName)
        {
            var vhost = new Vhost {Name = vhostName};
            var exchangeInfo = new ExchangeInfo(exchangeName, "direct");
            var exchange = managementClient.CreateExchange(exchangeInfo, vhost);
            return exchange;
        }

        [Fact]
        public void Should_be_able_to_create_an_exchange_with_plus_char_in_the_name()
        {
            var vhost = managementClient.GetVhost(vhostName);
            var exhangeInfo = new ExchangeInfo(testExchangetestQueueWithPlusChar, "direct");
            var queue = managementClient.CreateExchange(exhangeInfo, vhost);
            queue.Name.ShouldEqual(testExchangetestQueueWithPlusChar);
        }

        [Fact]
        public void Should_be_able_to_delete_an_exchange()
        {
            var exchange = managementClient.GetExchanges().SingleOrDefault(x => x.Name == testExchange);
            if (exchange == null)
            {
                throw new EasyNetQTestException(
                    string.Format("Test exchange '{0}' hasn't been created", testExchange));
            }

            managementClient.DeleteExchange(exchange);
        }

        [Fact]
        public void Should_be_able_to_delete_an_exchange_with_pluses()
        {
            var exchange = managementClient.GetExchanges().SingleOrDefault(x => x.Name == testExchangetestQueueWithPlusChar);
            if (exchange == null)
            {
                throw new EasyNetQTestException(
                    string.Format("Test exchange '{0}' hasn't been created", testExchange));
            }

            managementClient.DeleteExchange(exchange);
        }

        [Fact]
        public void Should_get_all_bindings_for_which_the_exchange_is_the_source()
        {
            var exchange = managementClient.GetExchanges().SingleOrDefault(x => x.Name == testExchange);
            if (exchange == null)
            {
                throw new EasyNetQTestException(
                    string.Format("Test exchange '{0}' hasn't been created", testExchange));
            }

            var bindings = managementClient.GetBindingsWithSource(exchange);

            foreach (var binding in bindings)
            {
                Console.Out.WriteLine("binding.RoutingKey = {0}", binding.RoutingKey);
            }
        }

        [Fact]
        public void Should_get_all_bindings_for_which_the_exchange_is_the_destination()
        {
            var exchange = managementClient.GetExchanges().SingleOrDefault(x => x.Name == testExchange);
            if (exchange == null)
            {
                throw new EasyNetQTestException(
                    string.Format("Test exchange '{0}' hasn't been created", testExchange));
            }

            var bindings = managementClient.GetBindingsWithDestination(exchange);

            foreach (var binding in bindings)
            {
                Console.Out.WriteLine("binding.RoutingKey = {0}", binding.RoutingKey);
            }
        }

        [Fact]
        public void Should_be_able_to_publish_to_an_exchange()
        {
            var exchange = managementClient.GetExchanges().SingleOrDefault(x => x.Name == testExchange);
            if (exchange == null)
            {
                throw new EasyNetQTestException(
                    string.Format("Test exchange '{0}' hasn't been created", testExchange));
            }

            var publishInfo = new PublishInfo(testQueue, "Hello World");
            var result = managementClient.Publish(exchange, publishInfo);

            // the testExchange isn't bound to a queue
            result.Routed.ShouldBeFalse();
        }

        [Fact]
        public void Should_get_queues()
        {
            var queues = managementClient.GetQueues();

            foreach (Queue queue in queues)
            {
                Console.Out.WriteLine("queue.Name = {0}", queue.Name);
            }
        }

        [Fact]
        public void Should_be_able_to_get_a_queue_by_name()
        {
            var vhost = new Vhost { Name = vhostName };
            var queue = managementClient.GetQueue(testQueue, vhost);
            queue.Name.ShouldEqual(testQueue);
        }

        [Fact]
        public void Should_be_able_to_get_a_queue_by_name_with_plus_char()
        {
            var vhost = new Vhost { Name = vhostName };
            var queue = managementClient.GetQueue(testQueueWithPlusChar, vhost);
            queue.Name.ShouldEqual(testQueueWithPlusChar);
        }

        [Fact]
        public void Should_be_able_to_get_a_queue_by_name_with_detailed_length_information()
        {
            var age = 60;
            var increment = 10;
            var vhost = new Vhost { Name = vhostName };
            var queue = managementClient.GetQueue(testQueue, vhost, lengthsCriteria: new GetLengthsCriteria(age, increment));
            queue.Name.ShouldEqual(testQueue);
            queue.MessagesDetails.Samples.Count.ShouldEqual(7);
            queue.MessagesReadyDetails.Samples.Count.ShouldEqual(7);
            queue.MessagesUnacknowledgedDetails.Samples.Count.ShouldEqual(7);
        }

        [Fact]
        public void Should_be_able_to_get_a_queue_by_name_with_detailed_rates_information()
        {
            var age = 60;
            var increment = 10;
            var vhost = new Vhost { Name = vhostName };
            var queue = managementClient.GetQueue(testQueue, vhost, ratesCriteria: new GetRatesCriteria(age, increment));
            queue.Name.ShouldEqual(testQueue);
            // All MessageStats are not always available, so tricky to test
            //queue.MessageStats.DeliverGetDetails.Samples.Count.ShouldEqual(7);
            //queue.MessageStats.GetNoAckDetails.Samples.Count.ShouldEqual(7);
            //queue.MessageStats.PublishDetails.Samples.Count.ShouldEqual(7);
        }

        [Fact]
        public void Should_be_able_to_get_a_queue_by_name_with_all_detailed_information()
        {
            var age = 60;
            var increment = 10;
            var vhost = new Vhost { Name = vhostName };
            var queue = managementClient.GetQueue(testQueue, vhost, lengthsCriteria: new GetLengthsCriteria(age, increment), ratesCriteria: new GetRatesCriteria(age, increment));
            queue.Name.ShouldEqual(testQueue);
            queue.MessagesDetails.Samples.Count.ShouldEqual(7);
            queue.MessagesReadyDetails.Samples.Count.ShouldEqual(7);
            queue.MessagesUnacknowledgedDetails.Samples.Count.ShouldEqual(7);
            // All MessageStats are not always available, so tricky to test
            //queue.MessageStats.DeliverGetDetails.Samples.Count.ShouldEqual(7);
            //queue.MessageStats.GetNoAckDetails.Samples.Count.ShouldEqual(7);
            //queue.MessageStats.PublishDetails.Samples.Count.ShouldEqual(7);
        }

        [Fact]
        public void Should_be_able_to_create_a_queue()
        {
            var vhost = managementClient.GetVhost(vhostName);
            var queueInfo = new QueueInfo(testQueue);
            var queue = managementClient.CreateQueue(queueInfo, vhost);
            queue = managementClient.GetQueue(testQueue, vhost);
            queue.Name.ShouldEqual(testQueue);
        }

        [Fact]
        public void Should_be_able_to_create_a_queue_with_plus_char_in_the_name()
        {
            var vhost = managementClient.GetVhost(vhostName);
            var queueInfo = new QueueInfo(testQueueWithPlusChar);
            var queue = managementClient.CreateQueue(queueInfo, vhost);
            queue.Name.ShouldEqual(testQueueWithPlusChar);
        }

        [Fact]
        public void Should_be_able_to_create_a_queue_with_arguments()
        {
            var exchangeName = "test-dead-letter-exchange";
            var argumentKey = "x-dead-letter-exchange";
            var vhost = managementClient.GetVhost(vhostName);
            var queueInfo = new QueueInfo(testQueue);
            queueInfo.Arguments.Add(argumentKey, exchangeName);
            var queue = managementClient.CreateQueue(queueInfo, vhost);
            queue.Arguments[argumentKey].ShouldNotBeNull();
            queue.Arguments[argumentKey].ShouldEqual(exchangeName);
        }

        [Fact]
        public void Should_be_able_to_delete_a_queue()
        {
            var queue = managementClient.GetQueues().SingleOrDefault(x => x.Name == testQueue);
            if (queue == null)
            {
                throw new EasyNetQTestException("Test queue has not been created");
            }

            managementClient.DeleteQueue(queue);
        }

        [Fact]
        public void Should_be_able_to_get_all_the_bindings_for_a_queue()
        {
            var queue = managementClient.GetQueues().SingleOrDefault(x => x.Name == testQueue);
            if (queue == null)
            {
                throw new EasyNetQTestException("Test queue has not been created");
            }

            var bindings = managementClient.GetBindingsForQueue(queue);

            foreach (var binding in bindings)
            {
                Console.Out.WriteLine("binding.RoutingKey = {0}", binding.RoutingKey);
            }
        }

        [Fact]
        public void Should_purge_a_queue()
        {
            var queue = managementClient.GetQueues().SingleOrDefault(x => x.Name == testQueue);
            if (queue == null)
            {
                throw new EasyNetQTestException("Test queue has not been created");
            }

            managementClient.Purge(queue);
        }

        [Fact]
        public void Should_be_able_to_get_messages_from_a_queue()
        {
            var queue = managementClient.GetQueues().SingleOrDefault(x => x.Name == testQueue);
            if (queue == null)
            {
                throw new EasyNetQTestException("Test queue has not been created");
            }

            var defaultExchange = new Exchange { Name = "amq.default", Vhost = vhostName };

            var properties = new Dictionary<string, string>
            {
                { "app_id", "management-test"}
            };

            var publishInfo = new PublishInfo(properties, testQueue, "Hello World", "string");

            managementClient.Publish(defaultExchange, publishInfo);

            var criteria = new GetMessagesCriteria(1, false);
            var messages = managementClient.GetMessagesFromQueue(queue, criteria);

            foreach (var message in messages)
            {
                Console.Out.WriteLine("message.Payload = {0}", message.Payload);
                foreach (var property in message.Properties)
                {
                    Console.Out.WriteLine("key: '{0}', value: '{1}'", property.Key, property.Value);
                }
            }
        }

        [Fact]
        public void Should_get_bindings()
        {
            var bindings = managementClient.GetBindings();

            foreach (var binding in bindings)
            {
                Console.Out.WriteLine("binding.Destination = {0}", binding.Destination);
                Console.Out.WriteLine("binding.Source = {0}", binding.Source);
                Console.Out.WriteLine("binding.PropertiesKey = {0}", binding.PropertiesKey);
            }
        }

        [Fact]
        public void Should_be_able_to_get_a_list_of_bindings_between_an_exchange_and_a_queue()
        {
            var queue = EnsureQueueExists(testQueue);

            var exchange = EnsureExchangeExists(testExchange);

            var bindings = managementClient.GetBindings(exchange, queue).ToArray();

            bindings.ShouldNotEqual(0);

            foreach (var binding in bindings)
            {
                Console.Out.WriteLine("binding.RoutingKey = {0}", binding.RoutingKey);
                Console.Out.WriteLine("binding.PropertiesKey = {0}", binding.PropertiesKey);
            }
        }

        private Queue EnsureQueueExists(string managementApiTestQueue)
        {
            return managementClient
                    .GetQueues()
                    .SingleOrDefault(x => x.Name == managementApiTestQueue) 
                    ?? CreateTestQueue(managementApiTestQueue);
        }

        private Queue CreateTestQueue(string queueName)
        {
            var vhost = managementClient.GetVhost(vhostName);
            var queueInfo = new QueueInfo(queueName);
            managementClient.CreateQueue(queueInfo, vhost);
            return managementClient.GetQueue(queueName, vhost);
        }

        [Fact]
        public void Should_be_able_to_get_a_list_of_bindings_between_an_exchange_and_an_exchange()
        {
            var exchange1 = EnsureExchangeExists(testExchange);
            var exchange2 = EnsureExchangeExists(testExchange2);

            var bindings = managementClient.GetBindings(exchange1, exchange2).ToArray();

            bindings.ShouldNotEqual(0);
            
            foreach (var binding in bindings)
            {
                Console.Out.WriteLine("binding.RoutingKey = {0}", binding.RoutingKey);
                Console.Out.WriteLine("binding.PropertiesKey = {0}", binding.PropertiesKey);
            }
        }

        private Exchange EnsureExchangeExists(string exchangeName)
        {
            return managementClient
                    .GetExchanges()
                    .SingleOrDefault(x => x.Name == exchangeName) 
                    ?? CreateExchange(exchangeName);
        }

        [Fact]
        public void Should_create_binding()
        {
            var vhost = managementClient.GetVhost(vhostName);
            var queue = managementClient.GetQueue(testQueue, vhost);
            var exchange = managementClient.GetExchange(testExchange, vhost);

            var bindingInfo = new BindingInfo(testQueue);

            managementClient.CreateBinding(exchange, queue, bindingInfo);
        }

        [Fact]
        public void Should_create_exchange_to_exchange_binding()
        {
            const string sourceExchangeName = "management_api_test_source_exchange";
            const string destinationExchangeName = "management_api_test_destination_exchange";

            var vhost = managementClient.GetVhost(vhostName);
            var sourceExchangeInfo = new ExchangeInfo(sourceExchangeName, "direct");
            var destinationExchangeInfo = new ExchangeInfo(destinationExchangeName, "direct");

            var sourceExchange = managementClient.CreateExchange(sourceExchangeInfo, vhost);
            var destinationExchange = managementClient.CreateExchange(destinationExchangeInfo, vhost);

            managementClient.CreateBinding(sourceExchange, destinationExchange, new BindingInfo("#"));

            var binding = managementClient.GetBindingsWithSource(sourceExchange).First();

            managementClient.DeleteExchange(sourceExchange);
            managementClient.DeleteExchange(destinationExchange);

            Assert.Equal("exchange", binding.DestinationType);
            Assert.Equal(destinationExchangeName, binding.Destination);
            Assert.Equal("#", binding.RoutingKey);
        }

        [Fact]
        public void Should_delete_binding()
        {
            var vhost = managementClient.GetVhost(vhostName);
            var queue = managementClient.GetQueue(testQueue, vhost);
            var exchange = managementClient.GetExchange(testExchange, vhost);

            var bindings = managementClient.GetBindings(exchange, queue);

            foreach (var binding in bindings)
            {
                managementClient.DeleteBinding(binding);
            }
        }

        [Fact]
        public void Should_get_vhosts()
        {
            var vhosts = managementClient.GetVHosts();

            foreach (var vhost in vhosts)
            {
                Console.Out.WriteLine("vhost.Name = {0}", vhost.Name);
            }
        }

        private const string testVHost = "management_test_virtual_host";

        [Fact]
        public void Should_be_able_to_get_an_individual_vhost()
        {
            var vhost = managementClient.GetVhost(testVHost);
            vhost.Name.ShouldEqual(testVHost);
        }

        [Fact]
        public void Should_create_a_virtual_host()
        {
            var vhost = managementClient.CreateVirtualHost(testVHost);
            vhost.Name.ShouldEqual(testVHost);
        }

        [Fact]
        public void Should_delete_a_virtual_host()
        {
            var vhost = managementClient.GetVhost(testVHost);
            managementClient.DeleteVirtualHost(vhost);
        }

        [Fact]
        public void Should_get_users()
        {
            var users = managementClient.GetUsers();

            foreach (var user in users)
            {
                Console.Out.WriteLine("user.Name = {0}", user.Name);
            }
        }

        [Fact]
        public void Should_be_able_to_get_a_user_by_name()
        {
            var user = managementClient.GetUser(testUser);
            user.Name.ShouldEqual(testUser);
        }

        [Fact]
        public void Should_be_able_to_create_a_user()
        {
            var userInfo = new UserInfo(testUser, "topSecret").AddTag("administrator");

            var user = managementClient.CreateUser(userInfo);
            user.Name.ShouldEqual(testUser);
        }

        [Fact]
        public void Should_be_able_to_create_a_user_with_the_policymaker_tag()
        {
            var userInfo = new UserInfo(testUser, "topSecret").AddTag("policymaker");

            var user = managementClient.CreateUser(userInfo);
            user.Name.ShouldEqual(testUser);
        }

        [Fact]
        public void Should_be_able_to_delete_a_user()
        {
            var user = managementClient.GetUser(testUser);
            managementClient.DeleteUser(user);
        }

        [Fact]
        public void Should_get_permissions()
        {
            var permissions = managementClient.GetPermissions();

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
        public void Should_be_able_to_create_permissions()
        {
            var user = managementClient.GetUsers().SingleOrDefault(x => x.Name == testUser);
            if (user == null)
            {
                throw new EasyNetQTestException(string.Format("user '{0}' hasn't been created", testUser));
            }
            var vhost = managementClient.GetVHosts().SingleOrDefault(x => x.Name == testVHost);
            if (vhost == null)
            {
                throw new EasyNetQTestException(string.Format("Test vhost: '{0}' has not been created", testVHost));
            }

            var permissionInfo = new PermissionInfo(user, vhost);
            managementClient.CreatePermission(permissionInfo);
        }

        [Fact]
        public void Should_be_able_to_create_permissions_in_default_Vhost()
        {
            var user = managementClient.GetUsers().SingleOrDefault(x => x.Name == testUser);
            if (user == null)
            {
                //create user if it does not exists
                var userInfo = new UserInfo(testUser, "topSecret").AddTag("administrator");
                user = managementClient.CreateUser(userInfo);
            }
            var vhost = managementClient.GetVHosts().SingleOrDefault(x => x.Name == vhostName);
            if (vhost == null)
            {
                throw new EasyNetQTestException(string.Format("Default vhost: '{0}' has not been created", testVHost));

            }

            var permissionInfo = new PermissionInfo(user, vhost);
            managementClient.CreatePermission(permissionInfo);
        }

        [Fact]
        public void Should_be_able_to_delete_permissions()
        {
            var permission = managementClient.GetPermissions()
                .SingleOrDefault(x => x.User == testUser && x.Vhost == testVHost);

            if (permission == null)
            {
                throw new EasyNetQTestException(string.Format("No permission for vhost: {0} and user: {1}",
                    testVHost, testUser));
            }

            managementClient.DeletePermission(permission);
        }

        [Fact]
        public void Should_be_able_to_change_the_password_of_a_user()
        {
            var userInfo = new UserInfo(testUser, "topSecret").AddTag("monitoring").AddTag("management");
            var user = managementClient.CreateUser(userInfo);

            var updatedUser = managementClient.ChangeUserPassword(testUser, "newPassword");

            updatedUser.Name.ShouldEqual(user.Name);
            updatedUser.Tags.ShouldEqual(user.Tags);
            updatedUser.PasswordHash.ShouldNotEqual(user.PasswordHash);
        }

        [Fact]
        public void Should_check_that_the_broker_is_alive()
        {
            var vhost = managementClient.GetVHosts().SingleOrDefault(x => x.Name == testVHost);
            if (vhost == null)
            {
                throw new EasyNetQTestException(string.Format("Test vhost: '{0}' has not been created", testVHost));
            }
            managementClient.IsAlive(vhost).ShouldBeTrue();
        }

        [Fact]
        public void Should_be_able_to_get_policies_list()
        {
            var policies = managementClient.GetPolicies();
            Assert.NotNull(policies);
        }

        [Fact]
        public void Should_be_able_to_create_policies()
        {
            var policyName = "asamplepolicy";
            var haMode = HaMode.All;
            var haSyncMode = HaSyncMode.Automatic;
            managementClient.CreatePolicy(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    HaMode = haMode,
                    HaSyncMode = haSyncMode
                }
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.ApplyTo == ApplyMode.All
                     && p.Definition.HaMode == haMode
                     && p.Definition.HaSyncMode == haSyncMode));
        }

        [Fact]
        public void Should_be_able_to_create_queues_only_policies()
        {
            var policyName = "asamplepolicy-queue-only";
            var haMode = HaMode.All;
            var haSyncMode = HaSyncMode.Automatic;
            managementClient.CreatePolicy(new Policy
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
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.ApplyTo == ApplyMode.Queues
                     && p.Definition.HaMode == haMode
                     && p.Definition.HaSyncMode == haSyncMode));
        }

        [Fact]
        public void Should_be_able_to_create_exchanges_only_policies()
        {
            var policyName = "asamplepolicy-exchange-only";
            var haMode = HaMode.All;
            var haSyncMode = HaSyncMode.Automatic;
            managementClient.CreatePolicy(new Policy
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
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.ApplyTo == ApplyMode.Exchanges
                     && p.Definition.HaMode == haMode
                     && p.Definition.HaSyncMode == haSyncMode));
        }

        [Fact]
        public void Should_be_able_to_create_alternate_exchange_policy()
        {
            var policyName = "a-sample-alternate-exchange-policy";
            var alternateExchange = "a-sample-alternate-exchange";
            managementClient.CreatePolicy(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    AlternateExchange = alternateExchange
                }
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.AlternateExchange == alternateExchange));
        }

        [Fact]
        public void Should_be_able_to_create_dead_letter_exchange_policy()
        {
            var policyName = "a-sample-dead-letter-exchange";
            var deadLetterExchange = "a-sample-dead-letter-exchange";
            var deadLetterRoutingKey = "a-sample-dead-letter-exchange-key";
            managementClient.CreatePolicy(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    DeadLetterExchange = deadLetterExchange,
                    DeadLetterRoutingKey = deadLetterRoutingKey
                }
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.DeadLetterExchange == deadLetterExchange
                     && p.Definition.DeadLetterRoutingKey == deadLetterRoutingKey));
        }

        [Fact]
        public void Should_be_able_to_create_message_ttl_policy()
        {
            var policyName = "a-sample-message-ttl";
            uint messageTtl = 5000;
            managementClient.CreatePolicy(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    MessageTtl = messageTtl
                }
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.MessageTtl == messageTtl));
        }

        [Fact]
        public void Should_be_able_to_create_expires_policy()
        {
            var policyName = "a-sample-expires";
            uint expires = 10000;
            managementClient.CreatePolicy(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    Expires = expires
                }
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.Expires == expires));
        }

        [Fact]
        public void Should_be_able_to_create_max_length_policy()
        {
            var policyName = "a-sample-max-length";
            uint maxLength = 500;
            managementClient.CreatePolicy(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    MaxLength = maxLength
                }
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.MaxLength == maxLength));
        }

        [Fact]
        public void Should_be_able_to_create_all_the_defitions_in_a_policy()
        {
            var policyName = "a-sample-all-definitions-in-a-policy";
            var priority = 999;
            var haMode = HaMode.All;
            var haSyncMode = HaSyncMode.Automatic;
            var alternateExchange = "a-sample-alternate-exchange";
            var deadLetterExchange = "a-sample-dead-letter-exchange";
            var deadLetterRoutingKey = "a-sample-dead-letter-exchange-key";
            uint messageTtl = 5000;
            uint expires = 10000;
            uint maxLength = 500;
            managementClient.CreatePolicy(new Policy
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
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(
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
        [Explicit]
        public void Should_be_able_to_create_federation_upstream_policy()
        {
            var policyName = "a-sample-federation-upstream-policy";
            
            managementClient.CreatePolicy(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    FederationUpstream = "my-upstream"
                }
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.FederationUpstream == "my-upstream"));
        }

        [Fact]
        [Explicit]
        public void Should_be_able_to_create_federation_upstream_set_policy()
        {
            var policyName = "a-sample-federation-upstream-set-policy";

            managementClient.CreatePolicy(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    FederationUpstreamSet = "my-upstream-set"
                }
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(
                p => p.Name == policyName
                     && p.Vhost == vhostName
                     && p.Definition.FederationUpstreamSet == "my-upstream-set"));
        }

        [Fact]
        public void Should_be_able_to_delete_policies()
        {
            var policyName = "asamplepolicy";
            managementClient.CreatePolicy(new Policy
            {
                Name = policyName,
                Pattern = "averyuncommonpattern",
                Vhost = vhostName,
                Definition = new PolicyDefinition
                {
                    HaMode = HaMode.All,
                    HaSyncMode = HaSyncMode.Automatic
                }
            });
            Assert.Equal(1, managementClient.GetPolicies().Count(p => p.Name == policyName && p.Vhost == vhostName));
            managementClient.DeletePolicy(policyName, new Vhost{Name = vhostName});
            Assert.Equal(0, managementClient.GetPolicies().Count(p => p.Name == policyName && p.Vhost == vhostName));
        }

        [Fact]
        public void Should_be_able_to_list_parameters()
        {
            var parameters = managementClient.GetParameters();
            Assert.NotNull(parameters);
        }

        [Fact(Skip = "Requires the federation plugin to work")]
        public void Should_be_able_to_create_parameter()
        {
            try
            {
                managementClient.DeleteParameter("federation-upstream", vhostName, "myfakefederationupstream");
            }
            catch (UnexpectedHttpStatusCodeException ex)
            {
                if (ex.StatusCodeNumber != 404)
                {
                    throw;
                }
            }
            
            managementClient.CreateParameter(new Parameter
            {
                Component = "federation-upstream",
                Name = "myfakefederationupstream",
                Vhost = vhostName,
                Value = new {uri = "amqp://guest:guest@localhost"}
            });
            Assert.True(managementClient.GetParameters().Where(p=>p.Name == "myfakefederationupstream").Any());
        }
    }
}

// ReSharper restore InconsistentNaming