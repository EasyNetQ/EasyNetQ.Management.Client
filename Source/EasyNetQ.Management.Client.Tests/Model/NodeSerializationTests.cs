// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.Tests.Model;

public class NodeSerializationTests
{
    private readonly List<Node> nodes;

    public NodeSerializationTests()
    {
        nodes = ResourceLoader.LoadObjectFromJson<List<Node>>("Nodes.json", ManagementClient.SerializerOptions);
    }

    [Fact]
    public void Should_load_one_node()
    {
        nodes.Count.Should().Be(1);
    }

    [Fact]
    public void Should_have_node_properties()
    {
        var node = nodes[0];

        node.Name.Should().Be("rabbit@THOMAS");
        node.Uptime.Should().Be(11463012619);
    }
}

// ReSharper restore InconsistentNaming
