namespace EasyNetQ.Management.Client.Model;

public record RatesCriteria(int MsgRatesAge, int MsgRatesIncr)
{
    public IReadOnlyDictionary<string, string> ToQueryParameters()
    {
        return new Dictionary<string, string>
        {
            { "msg_rates_age", MsgRatesAge.ToString() },
            { "msg_rates_incr", MsgRatesIncr.ToString() }
        };
    }
}
