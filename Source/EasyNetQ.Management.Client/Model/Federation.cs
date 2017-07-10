using System;
using EasyNetQ.Management.Client.Serialization;
using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Model
{
    public class Federation
    {
        public string Node { get; set; }
        public string Exchange { get; set; }
        public string UpstreamExchange { get; set; }
        public string Type { get; set; }
        public string Vhost { get; set; }
        public string Upstream { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public FederationStatus Status { get; set; }
        public string LocalConnection { get; set; }
        public string Uri { get; set; }

        public DateTime Timestamp { get; set; }
    }
}