using EasyNetQ.Management.Client.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Queue(
    string Name,
    string Vhost,

    QueueType Type,
    string? Node,
    string? State,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
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
) : QueueName(Name, Vhost)
{
    [JsonExtensionData()]
    public IDictionary<string, JsonElement>? JsonExtensionData { get; set; }

    [JsonIgnore()]
    public IReadOnlyDictionary<string, object?>? ExtensionData
    {
        get { return JsonExtensionDataExtensions.ToExtensionData(JsonExtensionData); }
        set { JsonExtensionData = JsonExtensionDataExtensions.ToJsonExtensionData(value); }
    }
};
