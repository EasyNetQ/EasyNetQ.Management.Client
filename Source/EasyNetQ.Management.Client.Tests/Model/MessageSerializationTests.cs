// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EasyNetQ.Management.Client.Tests.Model;

public class MessageSerializationTests
{
    [Fact]
    public void Should_be_able_to_deserialize_message_with_properties()
    {
        const string json = @"{""payload_bytes"":11,""redelivered"":true,""exchange"":""""," +
                            @"""routing_key"":""management_api_test_queue"",""message_count"":1," +
                            @"""properties"":{""delivery_mode"":2,""headers"":{""key"":""value""}},""payload"":""Hello World""," +
                            @"""payload_encoding"":""string""}";

        var message = JsonSerializer.Deserialize<Message>(json, ManagementClient.SerializerOptions);

        message.Properties.Count.Should().Be(2);
        message.Payload.Should().Be("Hello World");
        message.Properties["delivery_mode"].Should().Be(2);
        (message.Properties["headers"] as IReadOnlyDictionary<string, object>)["key"].Should().Be("value");
    }

    [Fact]
    public void Should_be_able_to_deserialize_message_without_properties()
    {
        const string json = @"{""payload_bytes"":11,""redelivered"":true,""exchange"":""""," +
                            @"""routing_key"":""management_api_test_queue"",""message_count"":1," +
                            @"""properties"":[],""payload"":""Hello World""," +
                            @"""payload_encoding"":""string""}";

        var message = JsonSerializer.Deserialize<Message>(json, ManagementClient.SerializerOptions);

        message.Properties.Count.Should().Be(0);
    }
}
