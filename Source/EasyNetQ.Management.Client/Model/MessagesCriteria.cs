namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class MessagesCriteria
{
    public long Count { get; private set; }
    public AckMode Ackmode { get; private set; }
    public string Encoding { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="count">
    /// Controls the number of messages to get. You may get fewer messages than this if the queue cannot immediately provide them.
    /// </param>
    /// <param name="ackMode">
    /// Determines if the message(s) should be placed back into the queue.
    /// </param>
    public MessagesCriteria(long count, AckMode ackMode)
    {
        Ackmode = ackMode;
        Count = count;
        Encoding = "auto";
    }
}
