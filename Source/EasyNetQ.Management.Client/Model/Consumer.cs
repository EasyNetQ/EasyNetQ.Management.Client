using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class Consumer
{
    public Dictionary<string, object> Arguments { get; set; }
    public bool AckRequired { get; set; }
    public bool Active { get; set; }
    public string ActivityStatus { get; set; }
    [JsonConverter(typeof(EmptyArrayAsDefaultConverter<ChannelDetail>))]
    public ChannelDetail ChannelDetails { get; set; }
    public string ConsumerTag { get; set; }
    public bool Exclusive { get; set; }
    public int PrefetchCount { get; set; }
    public QueueName Queue { get; set; }
}
