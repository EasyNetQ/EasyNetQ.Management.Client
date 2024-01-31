namespace EasyNetQ.Management.Client.Model;

public record PermissionInfo(
    string Configure = ".*",
    string Write = ".*",
    string Read = ".*"
)
{
    private const string DenyAll = "^$";
    private const string AllowAll = ".*";

    public PermissionInfo SetConfigure(string resourcesToAllow) => this with { Configure = resourcesToAllow };

    public PermissionInfo SetWrite(string resourcesToAllow) => this with { Write = resourcesToAllow };

    public PermissionInfo SetRead(string resourcesToAllow) => this with { Read = resourcesToAllow };

    public PermissionInfo DenyAllConfigure() => this with { Configure = DenyAll };

    public PermissionInfo DenyAllWrite() => this with { Write = DenyAll };

    public PermissionInfo DenyAllRead() => this with { Read = DenyAll };
}
