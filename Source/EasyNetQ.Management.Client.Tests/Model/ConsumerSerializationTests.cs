using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.Tests.Model;

public class ConsumerSerializationTests
{
    private readonly List<Consumer> consumers;

    public ConsumerSerializationTests()
    {
        consumers = ResourceLoader.LoadObjectFromJson<List<Consumer>>("Consumers.json", ManagementClient.SerializerOptions);
    }

    [Fact]
    public void Should_load_three_consumers()
    {
        consumers.Count.Should().Be(3);
    }

    [Fact]
    public void Should_have_consumer_properties()
    {
        var consumer = consumers[0];

        consumer.Queue.Name.Should().Be("Queue01");
        consumer.ChannelDetails.Node.Should().Be("rabbit@LOCALHOST");
    }
}
