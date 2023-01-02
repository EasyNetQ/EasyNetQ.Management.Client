using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class Channel
{
    public IReadOnlyList<ConsumerDetail> ConsumerDetails { get; set; }
    public ConnectionDetails ConnectionDetails { get; set; }

    [JsonConverter(typeof(EmptyArrayAsDefaultConverter<MessageStats>))]
    public MessageStats MessageStats { get; set; }

    public string IdleSince { get; set; }
    public bool Transactional { get; set; }
    public bool Confirm { get; set; }
    public int ConsumerCount { get; set; }
    public int MessagesUnacknowledged { get; set; }
    public int MessagesUnconfirmed { get; set; }
    public int MessagesUncommitted { get; set; }
    public int AcksUncommitted { get; set; }
    public int PrefetchCount { get; set; }
    public bool ClientFlowBlocked { get; set; }
    public string Node { get; set; }
    public string Name { get; set; }
    public int Number { get; set; }
    public string User { get; set; }
    public string Vhost { get; set; }
}

#nullable enable

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
