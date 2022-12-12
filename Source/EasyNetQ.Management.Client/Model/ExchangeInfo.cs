namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class ExchangeInfo
{
    public string Type { get; private set; }
    public bool AutoDelete { get; private set; }
    public bool Durable { get; private set; }
    public bool Internal { get; private set; }
    public Dictionary<string, string> Arguments { get; private set; }

    private readonly string name;

    private readonly ISet<string> exchangeTypes = new HashSet<string>
    {
        "direct", "topic", "fanout", "headers", "x-delayed-message"
    };

    public ExchangeInfo(string name, string type) : this(name, type, false, true, false, new Dictionary<string, string>())
    {
    }

    public ExchangeInfo(string name, string type, bool autoDelete, bool durable, bool @internal, Dictionary<string, string> arguments)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (!exchangeTypes.Contains(type))
        {
            throw new EasyNetQManagementException("Unknown exchange type '{0}', expected one of {1}",
                type,
                string.Join(", ", exchangeTypes));
        }

        this.name = name;
        Type = type;
        AutoDelete = autoDelete;
        Durable = durable;
        Internal = @internal;
        Arguments = arguments;
    }

    public string GetName()
    {
        return name;
    }
}
