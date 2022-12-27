#nullable disable
using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Model;

public class PolicyInfo
{
    public string Name { get; set; }
    public string Pattern { get; set; }
    public PolicyDefinition Definition { get; set; }
    [JsonProperty("apply-to", DefaultValueHandling = DefaultValueHandling.Include)]
    public ApplyMode ApplyTo { get; set; }
    public int Priority { get; set; }
}