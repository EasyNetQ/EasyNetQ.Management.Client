namespace EasyNetQ.Management.Client.Model
{
    public class QueueTotals
    {
        public int Messages { get; set; }
        public int MessagesReady { get; set; }
        public int MessagesUnacknowledged { get; set; }
        public LengthsDetails MessagesDetails { get; set; }
        public LengthsDetails MessagesReadyDetails { get; set; }
        public LengthsDetails MessagesUnacknowledgedDetails { get; set; }
    }
}