namespace EasyNetQ.Management.Client.Model;

public record TopicPermission(
    string User,
    string Vhost,
    string Exchange,
    string Write,
    string Read
);
