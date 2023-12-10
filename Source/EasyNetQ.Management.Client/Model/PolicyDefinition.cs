using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record PolicyDefinition
(
    // Queues [All types]
    [property: JsonPropertyName("max-length")]
    uint? MaxLength = null,
    [property: JsonPropertyName("max-length-bytes")]
    long? MaxLengthBytes = null,
    [property: JsonPropertyName("overflow")]
    Overflow? Overflow = null,
    [property: JsonPropertyName("expires")]
    uint? Expires = null,
    [property: JsonPropertyName("dead-letter-exchange")]
    string? DeadLetterExchange = null,
    [property: JsonPropertyName("dead-letter-routing-key")]
    string? DeadLetterRoutingKey = null,
    [property: JsonPropertyName("message-ttl")]
    uint? MessageTtl = null,
    [property: JsonPropertyName("consumer-timeout")]
    uint? ConsumerTimeout = null,

    // Queues [Classic]
    [property: JsonPropertyName("ha-mode")]
    HaMode? HaMode = null,
    [property: JsonPropertyName("ha-params")]
    HaParams? HaParams = null,
    [property: JsonPropertyName("ha-sync-mode")]
    HaSyncMode? HaSyncMode = null,
    [property: JsonPropertyName("ha-sync-batch-size")]
    int? HaSyncBatchSize = null,
    [property: JsonPropertyName("ha-promote-on-shutdown")]
    HaPromote? HaPromoteOnShutdown = null,
    [property: JsonPropertyName("ha-promote-on-failure")]
    HaPromote? HaPromoteOnFailure = null,
    [property: JsonPropertyName("queue-version")]
    uint? QueueVersion = null,
    [property: JsonPropertyName("queue-master-locator")]
    QueueLocator? QueueMasterLocator = null,

    // Queues [Quorum]
    [property: JsonPropertyName("delivery-limit")]
    uint? DeliveryLimit = null,
    [property: JsonPropertyName("dead-letter-strategy")]
    DeadLetterStrategy? DeadLetterStrategy = null,
    [property: JsonPropertyName("queue-leader-locator")]
    QueueLocator? QueueLeaderLocator = null,

    // Streams
    [property: JsonPropertyName("max-age")]
    string? MaxAge = null,
    [property: JsonPropertyName("stream-max-segment-size-bytes")]
    uint? StreamMaxSegmentSizeBytes = null,

    // Exchanges
    [property: JsonPropertyName("alternate-exchange")]
    string? AlternateExchange = null,

    // Federation
    [property: JsonPropertyName("federation-upstream")]
    string? FederationUpstream = null,
    [property: JsonPropertyName("federation-upstream-set")]
    string? FederationUpstreamSet = null,

    [property: JsonPropertyName("queue-mode")]
    string? QueueMode = null
);
