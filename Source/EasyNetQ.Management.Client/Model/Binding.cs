using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Binding(
    string Source,
    string Vhost,
    string Destination,
    string DestinationType,
    string RoutingKey,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?>? Arguments,
    string? PropertiesKey
);
