namespace EasyNetQ.Management.Client.Model;

/// <summary>
/// The criteria for retrieving messages from a queue
/// </summary>
public class GetMessagesCriteria
{
    public long Count { get; private set; }
    public string Ackmode { get; private set; }
    public string Encoding { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="count">
    /// Controls the number of messages to get. You may get fewer messages than this if the queue cannot immediately provide them.
    /// </param>
    /// <param name="ackModes">
    /// Determines if the message(s) should be placed back into the queue.
    /// </param>
    public GetMessagesCriteria(long count, AckModes ackModes)
    {
        Ackmode = ackModes.ToSnakeCaseString();
        Count = count;
        Encoding = "auto";
    }
}
