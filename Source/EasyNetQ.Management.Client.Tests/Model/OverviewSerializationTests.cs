using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.Tests.Model;

public class OverviewSerializationTests
{
    private readonly IReadOnlyList<Overview> overviews;

    public OverviewSerializationTests()
    {
        overviews = ResourceLoader.LoadObjectFromJson<IReadOnlyList<Overview>>("Overviews.json", ManagementClient.SerializerOptions);
    }

    [Fact]
    public void Should_deserialize()
    {
        overviews.Should().BeEquivalentTo(
            new Overview[]
            {
                new(
                    ManagementVersion: "3.11.5",
                    ExchangeTypes: new ExchangeTypeSpec[]
                    {
                        new(
                            Name: "direct",
                            Description: "AMQP direct exchange, as per the AMQP specification",
                            Enabled: true
                        ),
                        new(
                            Name: "fanout",
                            Description: "AMQP fanout exchange, as per the AMQP specification",
                            Enabled: true
                        ),
                        new(
                            Name: "headers",
                            Description: "AMQP headers exchange, as per the AMQP specification",
                            Enabled: true
                        ),
                        new(
                            Name: "topic",
                            Description: "AMQP topic exchange, as per the AMQP specification",
                            Enabled: true
                        )
                    },
                    RabbitmqVersion: "3.11.5",
                    ErlangVersion: "25.2",
                    MessageStats: new MessageStats(),
                    QueueTotals: new QueueTotals(),
                    ObjectTotals: new ObjectTotals(
                        Consumers: 0,
                        Queues: 0,
                        Exchanges: 7,
                        Connections: 0,
                        Channels: 0
                    ),
                    Node: "rabbit@localhost",
                    Listeners: new Listener[]
                    {
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "amqp",
                            IpAddress: "127.0.0.1",
                            Port: 5672,
                            SocketOpts: new SocketOpts(Backlog: 128, Nodelay: true, ExitOnClose: false)
                        ),
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "clustering",
                            IpAddress: "::",
                            Port: 25672
                        ),
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "http",
                            IpAddress: "::",
                            Port: 15672,
                            SocketOpts: new SocketOpts()
                        ),
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "mqtt",
                            IpAddress: "::",
                            Port: 1883,
                            SocketOpts: new SocketOpts(Backlog: 128, Nodelay: true)
                        ),
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "stomp",
                            IpAddress: "::",
                            Port: 61613,
                            SocketOpts: new SocketOpts(Backlog: 128, Nodelay: true)
                        ),
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "stream",
                            IpAddress: "::",
                            Port: 5552,
                            SocketOpts: new SocketOpts(Backlog: 128, Nodelay: true)
                        )
                    },
                    Contexts: Array.Empty<Context>()
                ),
                new(
                    ManagementVersion: "3.11.5",
                    ExchangeTypes: new ExchangeTypeSpec[]
                    {
                        new(
                            Name: "direct",
                            Description: "AMQP direct exchange, as per the AMQP specification",
                            Enabled: true
                        ),
                        new(
                            Name: "fanout",
                            Description: "AMQP fanout exchange, as per the AMQP specification",
                            Enabled: true
                        ),
                        new(
                            Name: "headers",
                            Description: "AMQP headers exchange, as per the AMQP specification",
                            Enabled: true
                        ),
                        new(
                            Name: "topic",
                            Description: "AMQP topic exchange, as per the AMQP specification",
                            Enabled: true
                        )
                    },
                    RabbitmqVersion: "3.11.5",
                    ErlangVersion: "25.2",
                    MessageStats: new MessageStats(),
                    QueueTotals: new QueueTotals(),
                    ObjectTotals: new ObjectTotals(
                        Consumers: 0,
                        Queues: 0,
                        Exchanges: 7,
                        Connections: 0,
                        Channels: 0
                    ),
                    Node: "rabbit@localhost",
                    Listeners: new Listener[]
                    {
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "amqp",
                            IpAddress: "127.0.0.1",
                            Port: 5672,
                            SocketOpts: new SocketOpts(Backlog: 128, Nodelay: true, ExitOnClose: false)
                        ),
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "clustering",
                            IpAddress: "::",
                            Port: 25672
                        ),
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "http",
                            IpAddress: "::",
                            Port: 15672,
                            SocketOpts: new SocketOpts()
                        ),
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "mqtt",
                            IpAddress: "::",
                            Port: 1883,
                            SocketOpts: new SocketOpts(Backlog: 128, Nodelay: true)
                        ),
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "stomp",
                            IpAddress: "::",
                            Port: 61613,
                            SocketOpts: new SocketOpts(Backlog: 128, Nodelay: true)
                        ),
                        new(
                            Node: "rabbit@localhost",
                            Protocol: "stream",
                            IpAddress: "::",
                            Port: 5552,
                            SocketOpts: new SocketOpts(Backlog: 128, Nodelay: true)
                        )
                    },
                    Contexts: new Context[]
                    {
                        new(
                            Node: "rabbit@localhost",
                            Description: "RabbitMQ Management",
                            Path: "/",
                            Port: 15672
                        )
                    }
                )
            },
            options => options
                .Excluding(o => o.ExtensionData)
                .Excluding(o => o.JsonExtensionData)
                .For(o => o.Listeners).Exclude(l => l.ExtensionData)
                .For(o => o.Listeners).Exclude(l => l.JsonExtensionData)
                .For(o => o.Listeners).Exclude(l => l.SocketOpts.ExtensionData)
                .For(o => o.Listeners).Exclude(l => l.SocketOpts.JsonExtensionData)
        );
    }
}
