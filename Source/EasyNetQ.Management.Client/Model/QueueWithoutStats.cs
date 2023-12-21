using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record QueueWithoutStats(
    string Name,
    string Vhost,

    QueueType Type,
    string? Node,
    string? State,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?> Arguments,
    bool Durable,
    bool Exclusive,
    bool AutoDelete,
    long MessagesReady,
    long MessagesUnacknowledged,
    long Messages
) : QueueName(Name, Vhost);
