namespace EasyNetQ.Management.Client.Model;

public record GetMessagesFromQueueInfo(long Count, AckMode Ackmode, string Encoding = "auto");
