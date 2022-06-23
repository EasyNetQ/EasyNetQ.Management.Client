using System.Collections.Generic;

namespace EasyNetQ.Management.Client.Model;

public class TopicPermissionInfo
{
    public string Exchange { get; private set; }
    public string Write { get; private set; }
    public string Read { get; private set; }

    private readonly User user;
    private readonly Vhost vhost;

    private const string denyAll = "^$";
    private const string allowAll = ".*";

    private readonly ISet<string> exchangeTypes = new HashSet<string>
    {
        "amq.direct", "amq.topic", "amq.fanout", "amq.headers", "amq.match", "amq.rabbitmq.trace"
    };

    public TopicPermissionInfo(User user, Vhost vhost)
    {
        this.user = user;
        this.vhost = vhost;

        Write = Read = allowAll;
    }

    public string GetUserName()
    {
        return user.Name;
    }

    public string GetVirtualHostName()
    {
        return vhost.Name;
    }

    public TopicPermissionInfo SetExchangeType(string exchangeType)
    {
        if (exchangeType != null && !exchangeTypes.Contains(exchangeType))
        {
            throw new EasyNetQManagementException("Unknown exchange type '{0}', expected one of {1}",
                exchangeType,
                string.Join(", ", exchangeTypes));
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
        Write = denyAll;
        return this;
    }

    public TopicPermissionInfo DenyAllRead()
    {
        Read = denyAll;
        return this;
    }
}
