namespace EasyNetQ.Management.Client.Model;

public class GetMessagesFromQueueInfo
{
    public long Count { get; }
    public AckMode Ackmode { get; }
    public string Encoding { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="count">
    /// Controls the number of messages to get. You may get fewer messages than this if the queue cannot immediately provide them.
    /// </param>
    /// <param name="ackMode">
    /// Determines if the message(s) should be placed back into the queue.
    /// </param>
    public GetMessagesFromQueueInfo(long count, AckMode ackMode)
    {
        Ackmode = ackMode;
        Count = count;
        Encoding = "auto";
    }
}
