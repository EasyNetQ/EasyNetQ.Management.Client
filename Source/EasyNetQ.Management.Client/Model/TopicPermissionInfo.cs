namespace EasyNetQ.Management.Client.Model;

public record TopicPermissionInfo(
    string Exchange,
    string Write = ".*",
    string Read = ".*"
)
{
    private const string DenyAll = "^$";
    private const string AllowAll = ".*";

    public TopicPermissionInfo SetWrite(string resourcesToAllow) => this with { Write = resourcesToAllow };
    public TopicPermissionInfo SetExchange(string exchange) => this with { Exchange = exchange };

    public TopicPermissionInfo SetRead(string resourcesToAllow) => this with { Read = resourcesToAllow };

    public TopicPermissionInfo DenyAllWrite() => this with { Write = DenyAll };

    public TopicPermissionInfo DenyAllRead() => this with { Read = DenyAll };
}
