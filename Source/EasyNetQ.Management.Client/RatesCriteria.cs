namespace EasyNetQ.Management.Client;

public record RatesCriteria(int MsgRatesAge, int MsgRatesIncr)
{
    public readonly IEnumerable<KeyValuePair<string, string>> QueryParameters =
        new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("msg_rates_age", MsgRatesAge.ToString()),
            new KeyValuePair<string, string>("msg_rates_incr", MsgRatesIncr.ToString())
        };
}
