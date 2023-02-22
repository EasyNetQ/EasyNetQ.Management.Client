using EasyNetQ.Management.Client.Serialization;
using NullGuard;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

/// <summary>
/// About shovel parameters: https://www.rabbitmq.com/shovel-dynamic.html
/// </summary>
[NullGuard(ValidationFlags.None)]
public record ParameterShovelValue(
    [property: JsonPropertyName("src-protocol")]
    string SrcProtocol,
    [property: JsonPropertyName("src-uri")]
    string SrcUri,
    [property: JsonPropertyName("src-exchange"), JsonConverter(typeof(TolerantStringConverter))]
    string SrcExchange,
    [property: JsonPropertyName("src-exchange-key"), JsonConverter(typeof(TolerantStringConverter))]
    string SrcExchangeKey,
    [property: JsonPropertyName("src-queue"), JsonConverter(typeof(TolerantStringConverter))]
    string SrcQueue,
    [property: JsonPropertyName("src-delete-after")]
    string SrcDeleteAfter,
    [property: JsonPropertyName("dest-protocol")]
    string DestProtocol,
    [property: JsonPropertyName("dest-uri")]
    string DestUri,
    [property: JsonPropertyName("dest-exchange"), JsonConverter(typeof(TolerantStringConverter))]
    string DestExchange,
    [property: JsonPropertyName("dest-queue"), JsonConverter(typeof(TolerantStringConverter))]
    string DestQueue,
    [property: JsonPropertyName("ack-mode")]
    string AckMode,
    [property: JsonPropertyName("add-forward-headers")]
    bool AddForwardHeaders
);
