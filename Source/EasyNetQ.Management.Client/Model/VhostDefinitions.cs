using System.Text.Json;
using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

/// <summary>
///     The definitions of one virtual host: its exchanges, queues, bindings, policies and parameters.
/// </summary>
public record VhostDefinitions(
    string? RabbitVersion = null,
    string? RabbitmqVersion = null,
    string? ProductName = null,
    string? ProductVersion = null,
    IReadOnlyList<ParameterDefinition>? Parameters = null,
    IReadOnlyList<PolicyDefinitionEntry>? Policies = null,
    IReadOnlyList<QueueDefinition>? Queues = null,
    IReadOnlyList<ExchangeDefinition>? Exchanges = null,
    IReadOnlyList<BindingDefinition>? Bindings = null
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
