namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class QueueInfo
{
    private readonly string name;

    public bool AutoDelete { get; private set; }
    public bool Durable { get; private set; }
    public Dictionary<string, object> Arguments { get; private set; }

    public QueueInfo(string name, bool autoDelete, bool durable, Dictionary<string, object> arguments)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        this.name = name;
        AutoDelete = autoDelete;
        Durable = durable;
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
    }

    public QueueInfo(string name) :
        this(name, false, true, new Dictionary<string, object>())
    {
    }

    public string GetName()
    {
        return name;
    }
}
