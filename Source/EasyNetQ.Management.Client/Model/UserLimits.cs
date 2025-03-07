using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record UserLimits(
    string User,
    [property: JsonPropertyName("value")]
    Limits Limits)
{
}
