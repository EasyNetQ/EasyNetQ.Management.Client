using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class PolicyDefinition
{
    [JsonPropertyName("ha-mode")]
    public HaMode? HaMode { get; set; }
    [JsonPropertyName("ha-params")]
    public HaParams HaParams { get; set; }
    [JsonPropertyName("ha-sync-mode")]
    public HaSyncMode? HaSyncMode { get; set; }
    [JsonPropertyName("ha-sync-batch-size")]
    public int? HaSyncBatchSize { get; set; }
    [JsonPropertyName("federation-upstream")]
    public string FederationUpstream { get; set; }
    [JsonPropertyName("federation-upstream-set")]
    public string FederationUpstreamSet { get; set; }
    [JsonPropertyName("alternate-exchange")]
    public string AlternateExchange { get; set; }
    [JsonPropertyName("dead-letter-exchange")]
    public string DeadLetterExchange { get; set; }
    [JsonPropertyName("dead-letter-routing-key")]
    public string DeadLetterRoutingKey { get; set; }
    [JsonPropertyName("queue-mode")]
    public string QueueMode { get; set; }
    [JsonPropertyName("message-ttl")]
    public uint? MessageTtl { get; set; }
    [JsonPropertyName("expires")]
    public uint? Expires { get; set; }
    [JsonPropertyName("max-length")]
    public uint? MaxLength { get; set; }
}
