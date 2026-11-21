using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record VhostLimits(
    string Vhost,
    [property: JsonPropertyName("value")]
    VhostLimitValues Limits
);

public record VhostLimitValues(
    [property: JsonPropertyName("max-connections")]
    long? MaxConnections = null,
    [property: JsonPropertyName("max-queues")]
    long? MaxQueues = null
);
