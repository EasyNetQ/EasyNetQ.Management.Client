namespace EasyNetQ.Management.Client.Model;

public class LengthsCriteria
{
    /// <summary>
    /// Create a new object for specifying queue length age and increment
    /// </summary>
    /// <param name="age">Age (in seconds) of oldest sample to return</param>
    /// <param name="increment">Interval (in seconds) between samples</param>
    public LengthsCriteria(int age, int increment)
    {
        LengthsAge = age;
        LengthsIncr = increment;
    }

    public int LengthsAge { get; }
    public int LengthsIncr { get; }

    public IReadOnlyDictionary<string, string> ToQueryParameters()
    {
        return new Dictionary<string, string>
        {
            { "lengths_age", LengthsAge.ToString() },
            { "lengths_incr", LengthsIncr.ToString() }
        };
    }
}
