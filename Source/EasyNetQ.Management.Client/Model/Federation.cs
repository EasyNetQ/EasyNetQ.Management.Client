namespace EasyNetQ.Management.Client.Model;

public record Federation(
    string Node,
    string Exchange,
    string UpstreamExchange,
    string Type,
    string Vhost,
    string Upstream,
    string Id,
    FederationStatus Status,
    string LocalConnection,
    string Uri,
    string Timestamp
);
