namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class ConnectionDetails
{
    public string Name { get; set; }
    public string PeerHost { get; set; }
    public int PeerPort { get; set; }
}
