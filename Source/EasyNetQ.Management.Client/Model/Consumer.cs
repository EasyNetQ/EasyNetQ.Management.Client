namespace EasyNetQ.Management.Client.Model
{
    public class Consumer
    {
        public Arguments Arguments { get; set; }
        public bool AckRequired { get; set;}
        public bool Active { get; set; }
        public string ActivityStatus { get; set; }
        public ChannelDetail ChannelDetails { get; set; }
        public string ConsumerTag { get; set; }
        public bool Exclusive { get; set; }
        public int PrefetchCount { get; set; }
        public QueueName Queue { get; set; }
    }
}
