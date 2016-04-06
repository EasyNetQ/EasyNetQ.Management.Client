using System.Collections.Generic;

namespace EasyNetQ.Management.Client.Model
{
    public class QueueEx : Queue
    {
        public MessagesDetails MessagesDetails { get; set; }
        public MessagesDetails MessagesReadyDetails { get; set; }
        public MessagesDetails MessagesUnacknowledgedDetails { get; set; }
    }
}