using System.Text.Json;
using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record BackingQueueStatus
(
    string? Mode,
    int Version,
    int Q1,
    int Q2,
    [property: JsonConverter(typeof(ObjectReadOnlyListConverter))]
    IReadOnlyList<object?> Delta,
    int Q3,
    int Q4,
    int Len,
    int PendingAcks,
    [property: JsonConverter(typeof(DoubleNamedFloatingPointLiteralsConverter))]
    double TargetRamCount,
    int RamMsgCount,
    int RamAckCount,
    long NextSeqId,
    long NextDeliverSeqId,
    int PersistentCount,
    double AvgIngressRate,
    double AvgEgressRate,
    double AvgAckIngressRate,
    double AvgAckEgressRate
)
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
