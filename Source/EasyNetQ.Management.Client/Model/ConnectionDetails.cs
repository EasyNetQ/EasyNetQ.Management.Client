namespace EasyNetQ.Management.Client.Model;

public record ConnectionDetails(
    string Name,
    string PeerHost,
    int PeerPort
);
