namespace EasyNetQ.Management.Client;

public record DeleteQueueCriteria(bool _IfUnused, bool _IfEmpty)
{
    public readonly IEnumerable<KeyValuePair<string, string>>? QueryParameters =
        (_IfUnused, _IfEmpty) switch
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

    public static readonly DeleteQueueCriteria IfUnused = new DeleteQueueCriteria(_IfUnused: true, _IfEmpty: false);
    public static readonly DeleteQueueCriteria IfEmpty = new DeleteQueueCriteria(_IfUnused: false, _IfEmpty: true);
    public static readonly DeleteQueueCriteria IfUnusedAndEmpty = new DeleteQueueCriteria(_IfUnused: true, _IfEmpty: true);
}
