using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record ConsumerArguments(
    [property: JsonPropertyName("x-credit")] CreditArgument? Credit
);

public record CreditArgument(int Credit, bool Drain);
