namespace EasyNetQ.Management.Client.Model;

#nullable disable

public record ConnectionDetails(
    string Name,
    string PeerHost,
    int PeerPort
);
