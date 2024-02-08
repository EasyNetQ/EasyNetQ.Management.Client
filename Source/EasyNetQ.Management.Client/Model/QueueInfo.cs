using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record QueueInfo(
    bool AutoDelete = false,
    bool Durable = true,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?>? Arguments = null
);
