namespace EasyNetQ.Management.Client;

public record LengthsCriteria(int LengthsAge, int LengthsIncr)
{
    public readonly IEnumerable<KeyValuePair<string, string>> QueryParameters =
        [
            new KeyValuePair<string, string>("lengths_age", LengthsAge.ToString()),
            new KeyValuePair<string, string>("lengths_incr", LengthsIncr.ToString())
        ];
}
