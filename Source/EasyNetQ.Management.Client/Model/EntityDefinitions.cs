using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

/// <summary>
///     A queue as a virtual host's definitions export and import it.
/// </summary>
public record QueueDefinition(
    string Name,
    bool Durable = true,
    bool AutoDelete = false,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?>? Arguments = null
);

/// <summary>
///     An exchange as a virtual host's definitions export and import it.
/// </summary>
public record ExchangeDefinition(
    string Name,
    string Type,
    bool Durable = true,
    bool AutoDelete = false,
    bool Internal = false,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?>? Arguments = null
);

/// <summary>
///     A binding as a virtual host's definitions export and import it.
/// </summary>
public record BindingDefinition(
    string Source,
    string Destination,
    string DestinationType,
    string RoutingKey,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?>? Arguments = null
);

/// <summary>
///     A parameter as a virtual host's definitions export and import it.
/// </summary>
public record ParameterDefinition(string Component, string Name, object Value);

/// <summary>
///     A policy as a virtual host's definitions export and import it.
/// </summary>
public record PolicyDefinitionEntry(
    string Name,
    string Pattern,
    PolicyDefinition Definition,
    [property: JsonPropertyName("apply-to")]
    ApplyMode ApplyTo = ApplyMode.All,
    int Priority = 0
);
