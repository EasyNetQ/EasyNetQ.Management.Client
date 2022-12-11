using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Model;

public class Federation
{
    public string Node { get; set; }
    public string Exchange { get; set; }
    public string UpstreamExchange { get; set; }
    public string Type { get; set; }
    public string Vhost { get; set; }
    public string Upstream { get; set; }

    public string Id { get; set; }
    public FederationStatus Status { get; set; }
    public string LocalConnection { get; set; }
    public string Uri { get; set; }

    public DateTime Timestamp { get; set; }
}
