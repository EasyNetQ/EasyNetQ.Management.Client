namespace EasyNetQ.Management.Client;

public record DeleteQueueCriteria(bool ifUnused, bool ifEmpty)
{
    public readonly IEnumerable<KeyValuePair<string, string>>? QueryParameters =
        (ifUnused, ifEmpty) switch
        {
            (false, false) => null,
            (false, true) => [
                new KeyValuePair<string, string>("if-empty", "true")
            ],
            (true, false) => [
                new KeyValuePair<string, string>("if-unused", "true")
            ],
            (true, true) => [
                new KeyValuePair<string, string>("if-unused", "true"),
                new KeyValuePair<string, string>("if-empty", "true")
            ]
        };

    public static readonly DeleteQueueCriteria IfUnused = new DeleteQueueCriteria(ifUnused: true, ifEmpty: false);
    public static readonly DeleteQueueCriteria IfEmpty = new DeleteQueueCriteria(ifUnused: false, ifEmpty: true);
    public static readonly DeleteQueueCriteria IfUnusedAndEmpty = new DeleteQueueCriteria(ifUnused: true, ifEmpty: true);
}
