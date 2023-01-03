namespace EasyNetQ.Management.Client.Model;

public record ExchangeTypeSpec(
    string Name,
    string Description,
    bool Enabled
);
