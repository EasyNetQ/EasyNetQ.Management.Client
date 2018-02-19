using Newtonsoft.Json;
using System.Collections.Generic;

namespace EasyNetQ.Management.Client.Model
{
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
}
