using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public class PublishInfo
{
    [JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    public IReadOnlyDictionary<string, object?> Properties { get; }
    public string RoutingKey { get; }
    public string Payload { get; }
    public PayloadEncoding PayloadEncoding { get; }

    public PublishInfo(IReadOnlyDictionary<string, object?> properties, string routingKey, string payload, PayloadEncoding payloadEncoding)
    {
        Properties = properties;
        RoutingKey = routingKey;
        Payload = payload;
        PayloadEncoding = payloadEncoding;
    }

    public PublishInfo(string routingKey, string payload)
        : this(new Dictionary<string, object?>(), routingKey, payload, PayloadEncoding.String)
    {
    }
}
