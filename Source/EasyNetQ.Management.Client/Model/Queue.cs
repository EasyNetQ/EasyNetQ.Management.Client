using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Queue(
    string Name,
    string Vhost,
    QueueType Type,
    string? Node,
    string? State,
    IReadOnlyDictionary<string, object?> Arguments,
    bool Durable,
    bool Exclusive,
    bool AutoDelete,
    long MessagesReady,
    long MessagesUnacknowledged,
    long Messages,

    long Memory,
    string? IdleSince,
    string? Policy,
    string? ExclusiveConsumerTag,
    long MessageBytes,
    long Consumers,
    long ActiveConsumers,
    BackingQueueStatus? BackingQueueStatus,
    IReadOnlyList<ConsumerDetail>? ConsumerDetails,
    long? HeadMessageTimestamp,
    IReadOnlyList<string>? SlaveNodes,
    IReadOnlyList<string>? SynchronisedSlaveNodes,
    LengthsDetails? MessagesDetails,
    LengthsDetails? MessagesReadyDetails,
    LengthsDetails? MessagesUnacknowledgedDetails,
    MessageStats? MessageStats
) : QueueWithoutStats(Name, Vhost, Type, Node, State, Arguments, Durable, Exclusive, AutoDelete, MessagesReady, MessagesUnacknowledged, Messages)
{
    [JsonExtensionData()]
    public Dictionary<string, object>? ExtensionData { get; set; }
};
