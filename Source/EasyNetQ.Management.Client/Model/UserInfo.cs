namespace EasyNetQ.Management.Client.Model;

public record UserInfo(string Name, string? Password, string? PasswordHash, IReadOnlyList<string> Tags)
{
    public static UserInfo ByPassword(string name, string password) => new(name, password, null, Array.Empty<string>());
    public static UserInfo ByPasswordHash(string name, string passwordHash) => new(name, null, passwordHash, Array.Empty<string>());

    public UserInfo AddTag(string tag)
        => this with { Tags = Tags.Contains(tag) ? Tags : new List<string>(Tags) { tag } };
}
