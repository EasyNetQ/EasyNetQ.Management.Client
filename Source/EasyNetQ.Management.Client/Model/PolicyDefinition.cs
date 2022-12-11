using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Model;

public class PolicyDefinition
{
    [JsonProperty("ha-mode")]
    public HaMode? HaMode;
    [JsonProperty("ha-params")]
    public HaParams HaParams;
    [JsonProperty("ha-sync-mode")]
    public HaSyncMode? HaSyncMode;
    [JsonProperty("federation-upstream")]
    public string FederationUpstream { get; set; }
    [JsonProperty("federation-upstream-set")]
    public string FederationUpstreamSet { get; set; }
    [JsonProperty("alternate-exchange")]
    public string AlternateExchange { get; set; }
    [JsonProperty("dead-letter-exchange")]
    public string DeadLetterExchange { get; set; }
    [JsonProperty("dead-letter-routing-key")]
    public string DeadLetterRoutingKey { get; set; }
    [JsonProperty("queue-mode")]
    public string QueueMode { get; set; }
    [JsonProperty("message-ttl")]
    public uint? MessageTtl { get; set; }
    [JsonProperty("expires")]
    public uint? Expires { get; set; }
    [JsonProperty("max-length")]
    public uint? MaxLength { get; set; }
}
