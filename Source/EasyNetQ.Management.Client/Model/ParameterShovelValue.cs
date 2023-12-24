using EasyNetQ.Management.Client.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

/// <summary>
/// About shovel parameters: https://www.rabbitmq.com/shovel-dynamic.html
/// </summary>
public record ParameterShovelValue(
    [property: JsonPropertyName("src-uri")]
    string SrcUri,
    [property: JsonPropertyName("dest-uri")]
    string DestUri,

    [property: JsonPropertyName("src-protocol")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? SrcProtocol = null, // Either amqp091 or amqp10. If omitted it will default to amqp091.
    [property: JsonPropertyName("src-queue")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? SrcQueue = null,
    [property: JsonPropertyName("src-queue-args")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?>? SrcQueueArguments = null,
    [property: JsonPropertyName("src-exchange")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? SrcExchange = null,
    [property: JsonPropertyName("src-exchange-key")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? SrcExchangeKey = null,

    [property: JsonPropertyName("src-delete-after")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? SrcDeleteAfter = null,
    [property: JsonPropertyName("src-prefetch-count")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    int? SrcPrefetchCount = null,

    [property: JsonPropertyName("dest-protocol")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? DestProtocol = null, // Either amqp091 or amqp10. If omitted it will default to amqp091.
    [property: JsonPropertyName("dest-queue")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? DestQueue = null,
    [property: JsonPropertyName("dest-queue-args")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?>? DestQueueArguments = null,
    [property: JsonPropertyName("dest-exchange")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? DestExchange = null,
    [property: JsonPropertyName("dest-exchange-key")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? DestExchangeKey = null,

    [property: JsonPropertyName("dest-add-forward-headers")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    bool? DestAddForwardHeaders = null,
    [property: JsonPropertyName("dest-add-timestamp-header")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    bool? DestAddTimestampHeader = null,

    [property: JsonPropertyName("ack-mode")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? AckMode = null,
    [property: JsonPropertyName("reconnect-delay")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    int? ReconnectDelay = null
)
{
    [JsonExtensionData()]
    public IDictionary<string, JsonElement>? JsonExtensionData { get; set; }

    [JsonIgnore()]
    public IReadOnlyDictionary<string, object?>? ExtensionData
    {
        get { return JsonExtensionDataExtensions.ToExtensionData(JsonExtensionData); }
        set { JsonExtensionData = JsonExtensionDataExtensions.ToJsonExtensionData(value); }
    }
};
