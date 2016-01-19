using System.Collections.Generic;
using EasyNetQ.Management.Client.Serialization;
using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Model
{
    public class Channel
    {
	    public List<ConsumerDetail> ConsumerDetails { get; set; }
		public ConnectionDetails ConnectionDetails { get; set; }
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

	public class ConsumerDetail
	{
		public Queue Queue { get; set; }
		public string ConsumerTag { get; set; }
		public bool Exclusive { get; set; }
		public bool AckRequired { get; set; }
		public Arguments Arguments { get; set; }
        public ChannelDetail ChannelDetails { get; set; }
	}

    public class ChannelDetail
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public string User { get; set; }
        public string ConnectionName { get; set; }
        [JsonConverter(typeof(TolerantInt32Converter))]
        public int PeerPort { get; set; }
        public string PeerHost { get; set; }
    }
}