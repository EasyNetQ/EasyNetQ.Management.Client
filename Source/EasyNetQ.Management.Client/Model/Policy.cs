using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Policy(
    string Name,
    string Pattern,
    PolicyDefinition Definition,
    string Vhost = "/",
    [property: JsonPropertyName("apply-to")]
    ApplyMode ApplyTo = ApplyMode.All,
    int Priority = 0
);
