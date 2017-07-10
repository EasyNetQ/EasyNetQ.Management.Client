using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Model
{
    public class Federation
    {
        public string Node { get; set; }
        public string Exchange { get; set; }
        public string Upstream_exchange { get; set; }
        public string Type { get; set; }
        public string Vhost { get; set; }
        public string Upstream { get; set; }
        public string Id { get; set; }
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public FederationStatus Status { get; set; }
        public string Local_connection { get; set; }
        public string Uri { get; set; }
        public string Timestamp { get; set; }
    }
}