namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class TopicPermissionInfo
{
    private static readonly ISet<string> ExchangeTypes = new HashSet<string>
    {
        "amq.direct", "amq.topic", "amq.fanout", "amq.headers", "amq.match", "amq.rabbitmq.trace"
    };

    private const string DenyAll = "^$";
    private const string AllowAll = ".*";

    public string UserName { get; private set; }
    public string Exchange { get; private set; }
    public string Write { get; private set; }
    public string Read { get; private set; }

    public TopicPermissionInfo(string userName)
    {
        UserName = userName;
        Write = Read = AllowAll;
    }

    public TopicPermissionInfo SetExchangeType(string exchangeType)
    {
        if (exchangeType != null && !ExchangeTypes.Contains(exchangeType))
        {
            throw new EasyNetQManagementException("Unknown exchange type '{0}', expected one of {1}",
                exchangeType,
                string.Join(", ", ExchangeTypes));
        }

        Exchange = exchangeType;
        return this;
    }

    public TopicPermissionInfo SetWrite(string resourcedToAllow)
    {
        Write = resourcedToAllow;
        return this;
    }

    public TopicPermissionInfo SetRead(string resourcesToAllow)
    {
        Read = resourcesToAllow;
        return this;
    }

    public TopicPermissionInfo DenyAllWrite()
    {
        Write = DenyAll;
        return this;
    }

    public TopicPermissionInfo DenyAllRead()
    {
        Read = DenyAll;
        return this;
    }
}
