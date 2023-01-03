using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Exchange(
    string Name,
    string Vhost,
    string Type,
    bool Durable = true,
    bool AutoDelete = false,
    bool Internal = false,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object>? Arguments = null
);
