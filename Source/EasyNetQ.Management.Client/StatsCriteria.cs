namespace EasyNetQ.Management.Client;

public record StatsCriteria(bool DisableStats, bool EnableQueueTotals)
{
    public readonly IEnumerable<KeyValuePair<string, string>>? QueryParameters =
        ((DisableStats, EnableQueueTotals)) switch
        {
            (true, false) => new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("disable_stats", "true")
            },
            (true, true) => new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("disable_stats", "true"),
                new KeyValuePair<string, string>("enable_queue_totals", "true")
            },
            (false, false) => null,
            (false, true) => throw new NotSupportedException()
        };

    public static StatsCriteria Disable = new StatsCriteria(true, false);
    public static StatsCriteria QueueTotalsOnly = new StatsCriteria(true, true);
}
