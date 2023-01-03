using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Message(
    int PayloadBytes,
    bool Redelivered,
    string Exchange,
    string RoutingKey,
    int MessageCount,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?> Properties,
    string Payload,
    string PayloadEncoding
);
