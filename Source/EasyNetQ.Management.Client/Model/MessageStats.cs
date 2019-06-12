using System;

namespace EasyNetQ.Management.Client.Model
{
    public class MessageStats
    {
        public Int64 Ack { get; set; }
        public MessageRateDetails AckDetails { get; set; }
        public long DeliverGet { get; set; }
        public MessageRateDetails DeliverGetDetails { get; set; }
        public long DeliverNoAck { get; set; }
        public MessageRateDetails DeliverNoAckDetails { get; set; }
        public long Publish { get; set; }
        public MessageRateDetails PublishDetails { get; set; }
        public long PublishIn { get; set; }
        public MessageRateDetails PublishInDetails { get; set; }
        public long PublishOut { get; set; }
        public MessageRateDetails PublishOutDetails { get; set; }
        public long Redeliver { get; set; }
        public MessageRateDetails RedeliverDetails { get; set; }
        public long Return { get; set; }
        public MessageRateDetails ReturnDetails { get; set; }
        public long Get { get; set; }
        public MessageRateDetails GetDetails { get; set; }
        public long GetNoAck { get; set; }
        public MessageRateDetails GetNoAckDetails { get; set; }
        public long Deliver { get; set; }
        public MessageRateDetails DeliverDetails { get; set; }
        public long Confirm { get; set; }
        public MessageRateDetails ConfirmDetails { get; set; }
    }
}
