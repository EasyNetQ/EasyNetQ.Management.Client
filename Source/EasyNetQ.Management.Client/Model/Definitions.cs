namespace EasyNetQ.Management.Client.Model;

public record Definitions(
    string RabbitVersion,
    IReadOnlyList<User> Users,
    IReadOnlyList<Vhost> Vhosts,
    IReadOnlyList<Permission> Permissions,
    IReadOnlyList<Queue> Queues,
    IReadOnlyList<Exchange> Exchanges,
    IReadOnlyList<Binding> Bindings
);
