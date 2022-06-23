// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;
using EasyNetQ.Management.Client.Serialization;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

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

        var message = JsonConvert.DeserializeObject<Message>(json, new PropertyConverter());

        message.Properties.Count.Should().Be(1);
        message.Payload.Should().Be("Hello World");
        message.Properties["delivery_mode"].Should().Be("2");
        message.Properties.Headers["key"].Should().Be("value");
    }

    [Fact]
    public void Should_be_able_to_deserialize_message_without_properties()
    {
        const string json = @"{""payload_bytes"":11,""redelivered"":true,""exchange"":""""," +
                            @"""routing_key"":""management_api_test_queue"",""message_count"":1," +
                            @"""properties"":[],""payload"":""Hello World""," +
                            @"""payload_encoding"":""string""}";

        var message = JsonConvert.DeserializeObject<Message>(json, new PropertyConverter());

        message.Properties.Count.Should().Be(0);
    }
}