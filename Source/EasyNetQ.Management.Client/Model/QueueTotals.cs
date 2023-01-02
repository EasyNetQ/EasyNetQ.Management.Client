namespace EasyNetQ.Management.Client.Model;

public record QueueTotals(
    long Messages = 0,
    long MessagesReady = 0,
    long MessagesUnacknowledged = 0,
    LengthsDetails? MessagesDetails = null,
    LengthsDetails? MessagesReadyDetails = null,
    LengthsDetails? MessagesUnacknowledgedDetails = null
);
