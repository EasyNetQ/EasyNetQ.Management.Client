using System.Text.Json;
using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.Tests.Serialization;

public class HaParamsConverterTests
{
    [Fact]
    public void Should_deserialize_exactly_count()
    {
        var deserializedObject = JsonSerializer.Deserialize<HaParams>("5", ManagementClient.SerializerOptions);
        Assert.NotNull(deserializedObject);
        Assert.Equal(HaMode.Exactly, deserializedObject.AssociatedHaMode);
        Assert.Equal(5L, deserializedObject.ExactlyCount);
    }

    [Fact]
    public void Should_deserialize_nodes_list()
    {
        var deserializedObject = JsonSerializer.Deserialize<HaParams>("[\"a\", \"b\"]", ManagementClient.SerializerOptions);
        Assert.NotNull(deserializedObject);
        Assert.Equal(HaMode.Nodes, deserializedObject.AssociatedHaMode);
        Assert.NotNull(deserializedObject.Nodes);
        Assert.Equal(2, deserializedObject.Nodes.Count);
        Assert.Contains("a", deserializedObject.Nodes);
        Assert.Contains("b", deserializedObject.Nodes);
    }

    [Fact]
    public void Should_be_able_to_serialize_count()
    {
        Assert.Equal("2", JsonSerializer.Serialize(new HaParams(AssociatedHaMode: HaMode.Exactly, ExactlyCount: 2), ManagementClient.SerializerOptions));
    }

    [Fact]
    public void Should_be_able_to_serialize_list()
    {
        Assert.Equal(
            JsonSerializer.Serialize(new[] { "a", "b" }),
            JsonSerializer.Serialize(new HaParams(AssociatedHaMode: HaMode.Nodes, Nodes: new[] { "a", "b" }), ManagementClient.SerializerOptions)
        );
    }
}
