namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class Context
{
    public string Node { get; set; }
    public string Description { get; set; }
    public string Path { get; set; }
    public int Port { get; set; }
}
