namespace EasyNetQ.Management.Client.Model;

public record BackingQueueStatus
(
    int Q1,
    int Q2,
    // TODO Custom converter is needed
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
