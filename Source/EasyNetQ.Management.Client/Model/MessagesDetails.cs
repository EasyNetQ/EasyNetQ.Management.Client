using System.Collections.Generic;

namespace EasyNetQ.Management.Client.Model
{
    public class MessagesDetails
    {
        public long Rate { get; set; }
        public long AvgRate { get; set; }
        public long Avg { get; set; }
        public List<MessagesDetailSample> Samples { get; set; }
    }
}