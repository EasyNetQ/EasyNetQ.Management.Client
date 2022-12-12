namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class TopicPermission
{
    public string User { get; set; }
    public string Vhost { get; set; }
    public string Exchange { get; set; }
    public string Write { get; set; }
    public string Read { get; set; }
}
