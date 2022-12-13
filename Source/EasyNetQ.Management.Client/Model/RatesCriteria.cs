namespace EasyNetQ.Management.Client.Model;

public class RatesCriteria
{
    /// <summary>
    /// Create a new object for specifying rate age and increment
    /// </summary>
    /// <param name="age">Age (in seconds) of oldest sample to return</param>
    /// <param name="increment">Interval (in seconds) between samples</param>
    public RatesCriteria(int age, int increment)
    {
        MsgRatesAge = age;
        MsgRatesIncr = increment;
    }

    public int MsgRatesAge { get; }
    public int MsgRatesIncr { get; }

    public IReadOnlyDictionary<string, string> ToQueryParameters()
    {
        return new Dictionary<string, string>
        {
            { "msg_rates_age", MsgRatesAge.ToString() },
            { "msg_rates_incr", MsgRatesIncr.ToString() }
        };
    }
}
