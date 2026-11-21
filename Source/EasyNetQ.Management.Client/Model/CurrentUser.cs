namespace EasyNetQ.Management.Client.Model;

public record CurrentUser(string Name, IReadOnlyList<string> Tags, bool? IsInternalUser = null);
