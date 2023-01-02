using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record BackingQueueStatus
(
    int Q1,
    int Q2,
    [property: JsonConverter(typeof(ObjectReadOnlyListConverter))]
    IReadOnlyList<object?> Delta,
    int Q3,
    int Q4,
    int Len,
    int PendingAcks,
    string TargetRamCount,
    int RamMsgCount,
    int RamAckCount,
    long NextSeqId,
    int PersistentCount,
    double AvgIngressRate,
    double AvgEgressRate,
    double AvgAckIngressRate,
    double AvgAckEgressRate
);
