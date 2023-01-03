using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record PolicyDefinition
(
    [property: JsonPropertyName("ha-mode")]
    HaMode? HaMode = null,
    [property: JsonPropertyName("ha-params")]
    HaParams? HaParams = null,
    [property: JsonPropertyName("ha-sync-mode")]
    HaSyncMode? HaSyncMode = null,
    [property: JsonPropertyName("ha-sync-batch-size")]
    int? HaSyncBatchSize = null,
    [property: JsonPropertyName("federation-upstream")]
    string? FederationUpstream = null,
    [property: JsonPropertyName("federation-upstream-set")]
    string? FederationUpstreamSet = null,
    [property: JsonPropertyName("alternate-exchange")]
    string? AlternateExchange = null,
    [property: JsonPropertyName("dead-letter-exchange")]
    string? DeadLetterExchange = null,
    [property: JsonPropertyName("dead-letter-routing-key")]
    string? DeadLetterRoutingKey = null,
    [property: JsonPropertyName("queue-mode")]
    string? QueueMode = null,
    [property: JsonPropertyName("message-ttl")]
    uint? MessageTtl = null,
    [property: JsonPropertyName("expires")]
    uint? Expires = null,
    [property: JsonPropertyName("max-length")]
    uint? MaxLength = null
);
