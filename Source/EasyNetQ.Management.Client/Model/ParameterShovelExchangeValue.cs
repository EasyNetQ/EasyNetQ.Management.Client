using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

/// <summary>
/// About shovel parameters: https://www.rabbitmq.com/shovel-dynamic.html
/// </summary>
public record ParameterShovelExchangeValue(
    [property: JsonPropertyName("src-uri")]
    string SrcUri,
    [property: JsonPropertyName("src-exchange")]
    string SrcExchange,
    [property: JsonPropertyName("src-delete-after")]
    string SrcDeleteAfter,
    [property: JsonPropertyName("dest-uri")]
    string DestUri,
    [property: JsonPropertyName("dest-exchange")]
    string DestExchange,
    [property: JsonPropertyName("ack-mode")]
    string AckMode,
    [property: JsonPropertyName("add-forward-headers")]
    bool AddForwardHeaders
);
