namespace EasyNetQ.Management.Client.Model;

public class ExchangeInfo
{
    public string Name { get; }
    public string Type { get; }
    public bool AutoDelete { get; }
    public bool Durable { get; }
    public bool Internal { get; }
    public Dictionary<string, string> Arguments { get; }

    public ExchangeInfo(
        string name,
        string type,
        bool autoDelete = false,
        bool durable = true,
        bool @internal = false,
        Dictionary<string, string>? arguments = null
    )
    {
        Name = name;
        Type = type;
        AutoDelete = autoDelete;
        Durable = durable;
        Internal = @internal;
        Arguments = arguments ?? new Dictionary<string, string>();
    }
}
