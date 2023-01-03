using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Queue(
    long Memory,
    string? IdleSince,
    string? Policy,
    string? ExclusiveConsumerTag,
    long MessagesReady,
    long MessagesUnacknowledged,
    long Messages,
    long Consumers,
    long ActiveConsumers,
    BackingQueueStatus? BackingQueueStatus,
    IReadOnlyList<ConsumerDetail>? ConsumerDetails,
    long? HeadMessageTimestamp,
    string Name,
    string Vhost,
    bool Durable,
    bool Exclusive,
    bool AutoDelete,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?> Arguments,
    string? Node,
    IReadOnlyList<string>? SlaveNodes,
    IReadOnlyList<string>? SynchronisedSlaveNodes,
    LengthsDetails? MessagesDetails,
    LengthsDetails? MessagesReadyDetails,
    LengthsDetails? MessagesUnacknowledgedDetails
);
