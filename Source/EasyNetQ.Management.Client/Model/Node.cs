namespace EasyNetQ.Management.Client.Model;


public record Node(
    string Name,
    string Type,
    bool Running,
    string? OsPid,
    long MemEts,
    long MemBinary,
    long MemProc,
    long MemProcUsed,
    long MemAtom,
    long MemAtomUsed,
    long MemCode,
    long FdUsed,
    long FdTotal,
    long MemUsed,
    long MemLimit,
    bool MemAlarm,
    long DiskFreeLimit,
    long DiskFree,
    bool DiskFreeAlarm,
    long ProcUsed,
    long ProcTotal,
    long Uptime,
    long RunQueue,
    long Processors,
    IReadOnlyList<ExchangeTypeSpec>? ExchangeTypes,
    IReadOnlyList<AuthMechanism>? AuthMechanisms,
    IReadOnlyList<Application>? Applications,
    IReadOnlyList<string>? Partitions
);
