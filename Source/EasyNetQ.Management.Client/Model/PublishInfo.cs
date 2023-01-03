using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record PublishInfo(
    string RoutingKey,
    string Payload,
    PayloadEncoding PayloadEncoding = PayloadEncoding.String,
    IReadOnlyDictionary<string, object?>? Properties = null
)
{
    [JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    public IReadOnlyDictionary<string, object?> Properties { get; init; } = Properties ?? new Dictionary<string, object?>();
}
