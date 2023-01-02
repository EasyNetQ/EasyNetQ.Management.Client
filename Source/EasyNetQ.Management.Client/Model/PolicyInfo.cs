#nullable disable
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public class PolicyInfo
{
    public string Name { get; set; }
    public string Pattern { get; set; }
    public PolicyDefinition Definition { get; set; }
    [JsonPropertyName("apply-to")]
    public ApplyMode ApplyTo { get; set; }
    public int Priority { get; set; }
}
