namespace EasyNetQ.Management.Client.Model;

public record VhostInfo(
    string? Description = null,
    string? Tags = null,
    QueueType? DefaultQueueType = null,
    bool? Tracing = null
);
