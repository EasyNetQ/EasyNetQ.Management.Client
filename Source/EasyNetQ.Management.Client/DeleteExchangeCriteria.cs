namespace EasyNetQ.Management.Client;

public record DeleteExchangeCriteria(bool _IfUnused)
{
    public IEnumerable<KeyValuePair<string, string>>? QueryParameters =
        _IfUnused switch
        {
            false => null,
            true => [
                new KeyValuePair<string, string>("if-unused", "true")
            ]
        };

    public static readonly DeleteExchangeCriteria IfUnused = new DeleteExchangeCriteria(_IfUnused: true);
}
