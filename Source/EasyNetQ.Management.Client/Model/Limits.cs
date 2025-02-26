using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Limits(
    [property: JsonPropertyName("max-channels")]
    long? MaxChannels,
    [property: JsonPropertyName("max-connections")]
    long? MaxConnections)
{
}
