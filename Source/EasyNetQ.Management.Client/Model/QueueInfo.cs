namespace EasyNetQ.Management.Client.Model;

public class QueueInfo
{
    public string Name { get; }
    public bool AutoDelete { get; }
    public bool Durable { get; }
    public Dictionary<string, object> Arguments { get; }

    public QueueInfo(
        string name,
        bool autoDelete = false,
        bool durable = true,
        Dictionary<string, object>? arguments = null
    )
    {
        Name = name;
        AutoDelete = autoDelete;
        Durable = durable;
        Arguments = arguments ?? new Dictionary<string, object>();
    }
}
