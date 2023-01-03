using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Serialization;

internal sealed class ObjectReadOnlyListConverter : JsonConverter<IReadOnlyList<object?>>
{
    public override IReadOnlyList<object?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonElement = JsonElement.ParseValue(ref reader);

        switch (jsonElement)
        {
            case { ValueKind: JsonValueKind.Array }:
                {
                    var list = new List<object?>(jsonElement.GetArrayLength());
                    foreach (var item in jsonElement.EnumerateArray())
                        list.Add(item.GetObjectValue());
                    return new ReadOnlyCollection<object?>(list);
                }
            default:
                throw new JsonException($"Expected array or object, but was {jsonElement.ValueKind}");
        }
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<object?> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var item in value)
            writer.WriteObjectValue(item);
        writer.WriteEndArray();
    }
}
