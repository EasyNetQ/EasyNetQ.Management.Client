namespace EasyNetQ.Management.Client.Model;

public record PermissionInfo(string UserName)
{
    private const string DenyAll = "^$";
    private const string AllowAll = ".*";

    public string Configure { get; private init; } = AllowAll;
    public string Write { get; private init; } = AllowAll;
    public string Read { get; private init; } = AllowAll;

    public PermissionInfo SetConfigure(string resourcesToAllow) => this with { Configure = resourcesToAllow };

    public PermissionInfo SetWrite(string resourcesToAllow) => this with { Write = resourcesToAllow };

    public PermissionInfo SetRead(string resourcesToAllow) => this with { Read = resourcesToAllow };

    public PermissionInfo DenyAllConfigure() => this with { Configure = DenyAll };

    public PermissionInfo DenyAllWrite() => this with { Write = DenyAll };

    public PermissionInfo DenyAllRead() => this with { Read = DenyAll };
}
