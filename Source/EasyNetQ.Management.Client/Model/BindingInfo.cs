namespace EasyNetQ.Management.Client.Model;

public class BindingInfo
{
    public string RoutingKey { get; private set; }
    public Dictionary<string, object> Arguments { get; private set; }

    public BindingInfo(string routingKey, Dictionary<string, object> arguments)
    {
        RoutingKey = routingKey;
        Arguments = arguments;
    }

    public BindingInfo(string routingKey)
        : this(routingKey, new Dictionary<string, object>())
    {
    }
}
