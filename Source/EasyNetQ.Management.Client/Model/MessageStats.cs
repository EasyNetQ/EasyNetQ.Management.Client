using System;

namespace EasyNetQ.Management.Client.Model
{
    public class MessageStats
    {
        public Int64 DeliverGet { get; set; }
        public MessageRateDetails DeliverGetDetails { get; set; }
        public Int64 DeliverNoAck { get; set; }
        public MessageRateDetails DeliverNoAckDetails { get; set; }
        public Int64 Publish { get; set; }
        public MessageRateDetails PublishDetails { get; set; }
        public Int64 PublishIn { get; set; }
        public MessageRateDetails PublishInDetails { get; set; }
        public Int64 PublishOut { get; set; }
        public MessageRateDetails PublishOutDetails { get; set; }
        public Int64 Redeliver { get; set; }
        public MessageRateDetails RedeliverDetails { get; set; }
        public Int64 Return { get; set; }
        public MessageRateDetails ReturnDetails { get; set; }
        public Int64 Get { get; set; }
        public MessageRateDetails GetDetails { get; set; }
        public Int64 GetNoAck { get; set; }
        public MessageRateDetails GetNoAckDetails { get; set; }
        public Int64 Deliver { get; set; }
        public MessageRateDetails DeliverDetails { get; set; }
        public Int64 Confirm { get; set; }
        public MessageRateDetails ConfirmDetails { get; set; }
    }
}
