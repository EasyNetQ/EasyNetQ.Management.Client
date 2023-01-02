namespace EasyNetQ.Management.Client.Model;

public record Permission(
    string User,
    string Vhost,
    string Configure,
    string Write,
    string Read
);
