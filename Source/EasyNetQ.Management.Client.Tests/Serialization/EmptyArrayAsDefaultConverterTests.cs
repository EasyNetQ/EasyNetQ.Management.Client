using System.Text.Json;
using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.Tests.Serialization;

public class EmptyArrayAsDefaultConverterTests
{
    private const string consumerDetailFormat =
        """{{"arguments":{0},"channel_details":{0},"ack_required":true,"active":true,"activity_status":"up","consumer_tag":"CT","consumer_timeout":"undefined","exclusive":false,"prefetch_count":0,"queue":{{"name":"Q","vhost":"V"}}}}""";

    private static readonly ConsumerDetail expectedConsumerDetail = new ConsumerDetail(new QueueName("Q", "V"), "CT", false, true, null, null);

    [Fact]
    public void Should_deserialize_empty_array()
    {
        var deserializedObject = JsonSerializer.Deserialize<ConsumerDetail>(string.Format(consumerDetailFormat, "[]"), ManagementClient.SerializerOptions);
        deserializedObject.Should().BeEquivalentTo(expectedConsumerDetail);
    }

    [Fact]
    public void Should_deserialize_empty_object()
    {
        var deserializedObject = JsonSerializer.Deserialize<ConsumerDetail>(string.Format(consumerDetailFormat, "{}"), ManagementClient.SerializerOptions);
        deserializedObject.Should().BeEquivalentTo(expectedConsumerDetail);
    }

    [Fact]
    public void Should_deserialize_null()
    {
        var deserializedObject = JsonSerializer.Deserialize<ConsumerDetail>(string.Format(consumerDetailFormat, "null"), ManagementClient.SerializerOptions);
        deserializedObject.Should().BeEquivalentTo(expectedConsumerDetail);
    }

    [Fact]
    public void Should_throw_deserialize_non_empty_array()
    {
        var action = () => JsonSerializer.Deserialize<ConsumerDetail>(string.Format(consumerDetailFormat, "[1]"), ManagementClient.SerializerOptions);
        action.Should().ThrowExactly<JsonException>();
    }

    [Fact]
    public void Should_throw_deserialize_string()
    {
        var action = () => JsonSerializer.Deserialize<ConsumerDetail>(string.Format(consumerDetailFormat, "\"consumer_detail\""), ManagementClient.SerializerOptions);
        action.Should().ThrowExactly<JsonException>();
    }
}
