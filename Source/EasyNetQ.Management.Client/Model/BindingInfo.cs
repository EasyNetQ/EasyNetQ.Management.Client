namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class BindingInfo
{
    public string RoutingKey { get; set; }
    public Dictionary<string, object> Arguments { get; set; }

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
