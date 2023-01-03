namespace EasyNetQ.Management.Client.Model;

public record User(
    string Name,
    string PasswordHash,
    IReadOnlyList<string> Tags
);
