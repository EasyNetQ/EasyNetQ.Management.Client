using System;
using System.Collections.Generic;

namespace EasyNetQ.Management.Client.Model
{
    public class MessagesDetails
    {
        public long Rate { get; set; }

        [Obsolete("The Interval and LastEvent properties were removed in RabbitMQ v3.1")]
        public long Interval { get; set; }
        [Obsolete("The Interval and LastEvent properties were removed in RabbitMQ v3.1")]
        public long LastEvent { get; set; }

        public long AvgRate { get; set; }
        public long Avg { get; set; }

        public List<MessagesDetailSample> Samples { get; set; }
    }
}