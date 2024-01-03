using EasyNetQ.Management.Client.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record ShovelStatus
(
    string Name,
    string Vhost,
    string Node,
    [property: JsonConverter(typeof(DateTimeConverter))]
    DateTime Timestamp,
    string Type,
    string State,

    string? SrcProtocol = null,
    string? SrcUri = null,
    string? SrcQueue = null,
    string? SrcExchange = null,
    string? SrcExchangeKey = null,
    string? DestProtocol = null,
    string? DestUri = null,
    string? DestQueue = null,
    string? DestExchange = null,
    string? DestExchangeKey = null,
    string? BlockedStatus = null,

    string? Reason = null
)
{
    [JsonExtensionData()]
    public IDictionary<string, JsonElement>? JsonExtensionData { get; set; }

    [JsonIgnore()]
    public IReadOnlyDictionary<string, object?>? ExtensionData
    {
        get { return JsonExtensionDataExtensions.ToExtensionData(JsonExtensionData); }
        set { JsonExtensionData = JsonExtensionDataExtensions.ToJsonExtensionData(value); }
    }
};
