using EasyNetQ.Management.Client.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Overview(
    string ManagementVersion,
    IReadOnlyList<ExchangeTypeSpec> ExchangeTypes,
    string RabbitmqVersion,
    string ErlangVersion,
    MessageStats MessageStats,
    QueueTotals QueueTotals,
    ObjectTotals ObjectTotals,
    string Node,
    IReadOnlyList<Listener> Listeners,
    IReadOnlyList<Context> Contexts
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

public record ObjectTotals(
    int Consumers,
    int Queues,
    int Exchanges,
    int Connections,
    int Channels
);
