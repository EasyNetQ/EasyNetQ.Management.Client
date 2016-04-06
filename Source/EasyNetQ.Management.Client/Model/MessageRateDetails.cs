using System;
using System.Collections.Generic;

namespace EasyNetQ.Management.Client.Model
{
    public class MessageRateDetails
    {
        public double Rate { get; set; }

        [Obsolete("The Interval and LastEvent properties were removed in RabbitMQ v3.1")]
        public long Interval { get; set; }
        [Obsolete("The Interval and LastEvent properties were removed in RabbitMQ v3.1")]
        public long LastEvent { get; set; }

        public double AvgRate { get; set; }
        public double Avg { get; set; }

        public List<MessageRateSample> Samples { get; set; }
    }

}