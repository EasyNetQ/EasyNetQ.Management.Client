namespace EasyNetQ.Management.Client.Model;

public record MessageStats(
    long Ack = 0,
    MessageRateDetails? AckDetails = null,
    long DeliverGet = 0,
    MessageRateDetails? DeliverGetDetails = null,
    long DeliverNoAck = 0,
    MessageRateDetails? DeliverNoAckDetails = null,
    long Publish = 0,
    MessageRateDetails? PublishDetails = null,
    long PublishIn = 0,
    MessageRateDetails? PublishInDetails = null,
    long PublishOut = 0,
    MessageRateDetails? PublishOutDetails = null,
    long Redeliver = 0,
    MessageRateDetails? RedeliverDetails = null,
    long Return = 0,
    MessageRateDetails? ReturnDetails = null,
    long Get = 0,
    MessageRateDetails? GetDetails = null,
    long GetNoAck = 0,
    MessageRateDetails? GetNoAckDetails = null,
    long Deliver = 0,
    MessageRateDetails? DeliverDetails = null,
    long Confirm = 0,
    MessageRateDetails? ConfirmDetails = null
);
