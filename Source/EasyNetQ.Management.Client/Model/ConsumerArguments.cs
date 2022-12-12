using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class ConsumerArguments
{
    [JsonProperty(PropertyName = "x-credit")]
    public CreditArgument Credit { get; set; }
}

public class CreditArgument
{
    public int Credit { get; set; }
    public bool Drain { get; set; }
}
