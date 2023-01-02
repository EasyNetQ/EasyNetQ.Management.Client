using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Consumer(
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?> Arguments,
    bool AckRequired,
    bool Active,
    string ActivityStatus,
    [property: JsonConverter(typeof(EmptyArrayAsDefaultConverter<ChannelDetail>))]
    ChannelDetail ChannelDetails,
    string ConsumerTag,
    bool Exclusive,
    int PrefetchCount,
    QueueName Queue
);
