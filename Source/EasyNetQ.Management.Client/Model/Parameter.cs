namespace EasyNetQ.Management.Client.Model;

public record Parameter(
    string Vhost,
    string Component,
    string Name,
    object Value
);
