using System;
using System.Threading.Tasks;
using EasyNetQ.Management.Client.Model;
using Xunit;

namespace EasyNetQ.Management.Client.IntegrationTests;

[Collection("Rabbitmq collection")]
public class ScenarioTest
{
    public ScenarioTest(RabbitMqFixture fixture)
    {
        rabbitMqUrl = $"http://{fixture.RabbitHostForManagement}";
    }

    private readonly string rabbitMqUrl;

    /// <summary>
    ///     Demonstrate how to create a virtual host, add some users, set permissions
    ///     and create exchanges, queues and bindings.
    /// </summary>
    [Fact]
    public async Task Should_be_able_to_provision_a_virtual_host()
    {
        var initial = new ManagementClient(rabbitMqUrl, Configuration.RabbitMqUser,
            Configuration.RabbitMqPassword, Configuration.RabbitMqManagementPort);

        // first create a new virtual host
        var vhost = await initial.CreateVhostAsync("my_virtual_host").ConfigureAwait(false);

        // next create a user for that virutal host
        var user = await initial.CreateUserAsync(new UserInfo("mike", "topSecret").AddTag("administrator"))
            .ConfigureAwait(false);

        // give the new user all permissions on the virtual host
        await initial.CreatePermissionAsync(new PermissionInfo(user, vhost)).ConfigureAwait(false);

        // now log in again as the new user
        var management = new ManagementClient(rabbitMqUrl, user.Name, "topSecret",
            Configuration.RabbitMqManagementPort);

        // test that everything's OK
        await management.IsAliveAsync(vhost).ConfigureAwait(false);

        // create an exchange
        var exchange = await management.CreateExchangeAsync(new ExchangeInfo("my_exchagne", "direct"), vhost)
            .ConfigureAwait(false);

        // create a queue
        var queue = await management.CreateQueueAsync(new QueueInfo("my_queue"), vhost).ConfigureAwait(false);

        // bind the exchange to the queue
        await management.CreateBindingAsync(exchange, queue, new BindingInfo("my_routing_key"))
            .ConfigureAwait(false);

        // publish a test message
        await management.PublishAsync(exchange, new PublishInfo("my_routing_key", "Hello World!"))
            .ConfigureAwait(false);

        // get any messages on the queue
        var messages = await management
            .GetMessagesFromQueueAsync(queue, new GetMessagesCriteria(1, Ackmodes.ack_requeue_false))
            .ConfigureAwait(false);

        foreach (var message in messages) Console.Out.WriteLine("message.payload = {0}", message.Payload);
    }
}
