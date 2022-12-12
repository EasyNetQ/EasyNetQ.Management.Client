namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class Listener
{
    public string Node { get; set; }
    public string Protocol { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
}
