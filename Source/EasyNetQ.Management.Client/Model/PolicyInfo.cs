using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record PolicyInfo(
    string Pattern,
    PolicyDefinition Definition,
    [property: JsonPropertyName("apply-to")]
    ApplyMode ApplyTo,
    int Priority
);
