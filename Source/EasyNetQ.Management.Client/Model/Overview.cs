namespace EasyNetQ.Management.Client.Model;

public record Overview(
    string ManagementVersion,
    IReadOnlyList<ExchangeTypeSpec> ExchangeTypes,
    string RabbitmqVersion,
    string ErlangVersion,
    MessageStats MessageStats,
    QueueTotals QueueTotals,
    ObjectTotals ObjectTotals,
    string Node,
    IReadOnlyList<Listener> Listeners,
    IReadOnlyList<Context> Contexts
);

public record ObjectTotals(
    int Consumers,
    int Queues,
    int Exchanges,
    int Connections,
    int Channels
);
