using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record ExchangeInfo(
    string Name,
    string Type,
    bool AutoDelete = false,
    bool Durable = true,
    bool Internal = false,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object>? Arguments = null
);
