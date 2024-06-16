namespace EasyNetQ.Management.Client;

public record DeleteExchangeCriteria(bool ifUnused)
{
    public IEnumerable<KeyValuePair<string, string>>? QueryParameters =
        ifUnused switch
        {
            false => null,
            true => [
                new KeyValuePair<string, string>("if-unused", "true")
            ]
        };

    public static readonly DeleteExchangeCriteria IfUnused = new DeleteExchangeCriteria(ifUnused: true);
}
