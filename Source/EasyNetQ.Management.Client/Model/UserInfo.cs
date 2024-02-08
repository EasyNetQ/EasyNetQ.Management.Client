namespace EasyNetQ.Management.Client.Model;

public record UserInfo(
    string? Password,
    string? PasswordHash,
    IReadOnlyList<string> Tags
)
{
    public static UserInfo ByPassword(string password) => new(password, null, Array.Empty<string>());
    public static UserInfo ByPasswordHash(string passwordHash) => new(null, passwordHash, Array.Empty<string>());

    public UserInfo AddTag(string tag)
        => this with { Tags = Tags.Contains(tag) ? Tags : new List<string>(Tags) { tag } };
}
