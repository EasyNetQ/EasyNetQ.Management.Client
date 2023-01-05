using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Serialization;

internal sealed class StringObjectReadOnlyDictionaryConverter : JsonConverter<IReadOnlyDictionary<string, object?>>
{
    public override IReadOnlyDictionary<string, object?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonElement = JsonElement.ParseValue(ref reader);

        switch (jsonElement)
        {
            case { ValueKind: JsonValueKind.Array } when jsonElement.GetArrayLength() > 0:
                throw new JsonException("Empty array is required");
            case { ValueKind: JsonValueKind.Array }:
                return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>());
            case { ValueKind: JsonValueKind.Object }:
                {
                    var dictionary = new Dictionary<string, object?>();
                    foreach (var property in jsonElement.EnumerateObject())
                        dictionary.Add(property.Name, property.Value.GetObjectValue());
                    return new ReadOnlyDictionary<string, object?>(dictionary);
                }
            default:
                throw new JsonException($"Expected array or object, but was {jsonElement.ValueKind}");
        }
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyDictionary<string, object?> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var kvp in value)
        {
            writer.WritePropertyName(kvp.Key);
            writer.WriteObjectValue(kvp.Value);
        }
        writer.WriteEndObject();
    }
}
