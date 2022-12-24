namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class PermissionInfo
{
    private const string DenyAll = "^$";
    private const string AllowAll = ".*";

    public string UserName { get; private set; }
    public string Configure { get; private set; }
    public string Write { get; private set; }
    public string Read { get; private set; }

    public PermissionInfo(string userName)
    {
        UserName = userName;
        Configure = Write = Read = AllowAll;
    }

    public PermissionInfo SetConfigure(string resourcesToAllow)
    {
        Configure = resourcesToAllow;
        return this;
    }

    public PermissionInfo SetWrite(string resourcedToAllow)
    {
        Write = resourcedToAllow;
        return this;
    }

    public PermissionInfo SetRead(string resourcesToAllow)
    {
        Read = resourcesToAllow;
        return this;
    }

    public PermissionInfo DenyAllConfigure()
    {
        Configure = DenyAll;
        return this;
    }

    public PermissionInfo DenyAllWrite()
    {
        Write = DenyAll;
        return this;
    }

    public PermissionInfo DenyAllRead()
    {
        Read = DenyAll;
        return this;
    }
}
