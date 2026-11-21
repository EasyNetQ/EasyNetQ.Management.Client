using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

/// <summary>
///     A node's memory use in bytes by category (connection_readers, quorum_queue_procs, code, ...), plus its "total" and "strategy".
/// </summary>
public record NodeMemory(
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?> Memory
);
