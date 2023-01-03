using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Channel(
    IReadOnlyList<ConsumerDetail>? ConsumerDetails,
    ConnectionDetails? ConnectionDetails,
    [property: JsonConverter(typeof(EmptyArrayAsDefaultConverter<MessageStats>))]
    MessageStats? MessageStats,
    string? IdleSince,
    bool Transactional,
    bool Confirm,
    int ConsumerCount,
    int MessagesUnacknowledged,
    int MessagesUnconfirmed,
    int MessagesUncommitted,
    int AcksUncommitted,
    int PrefetchCount,
    bool ClientFlowBlocked,
    string Node,
    string Name,
    int Number,
    string User,
    string Vhost
);

public record ConsumerDetail(
    QueueName Queue,
    string ConsumerTag,
    bool Exclusive,
    bool AckRequired,
    [property: JsonConverter(typeof(EmptyArrayAsDefaultConverter<ConsumerArguments>))]
    ConsumerArguments? Arguments,
    [property: JsonConverter(typeof(EmptyArrayAsDefaultConverter<ChannelDetail>))]
    ChannelDetail? ChannelDetails
);

public record ChannelDetail(
    string Name,
    [property: JsonConverter(typeof(TolerantInt32Converter))]
    int Number,
    string User,
    string ConnectionName,
    [property: JsonConverter(typeof(TolerantInt32Converter))]
    int PeerPort,
    string PeerHost,
    string? Node
);
