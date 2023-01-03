namespace EasyNetQ.Management.Client.Model;

public record LengthsCriteria(int LengthsAge, int LengthsIncr)
{
    public IReadOnlyDictionary<string, string> ToQueryParameters()
    {
        return new Dictionary<string, string>
        {
            { "lengths_age", LengthsAge.ToString() },
            { "lengths_incr", LengthsIncr.ToString() }
        };
    }
}
