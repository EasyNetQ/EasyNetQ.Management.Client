// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using EasyNetQ.Management.Client.Model;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model
{
    public class NodeSerializationTests
    {
        private readonly List<Node> nodes;

        public NodeSerializationTests()
        {
            nodes = ResourceLoader.LoadObjectFromJson<List<Node>>("Nodes.json");
        }

        [Fact]
        public void Should_load_one_node()
        {
            nodes.Count.ShouldEqual(1);
        }

        [Fact]
        public void Should_have_node_properties()
        {
            var node = nodes[0];

            node.Name.ShouldEqual("rabbit@THOMAS");
            node.Uptime.ShouldEqual(11463012619);
        }
    }
}

// ReSharper restore InconsistentNaming