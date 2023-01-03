namespace EasyNetQ.Management.Client.Model;

public record Listener(
    string Node,
    string Protocol,
    string IpAddress,
    int Port
);
