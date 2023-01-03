using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Serialization;

internal sealed class EmptyArrayAsDefaultConverter<T> : JsonConverter<T>
{
    // From http://stackoverflow.com/questions/17171737/how-to-deserialize-json-data-which-sometimes-is-an-empty-array-and-sometimes-a-s
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonElement = JsonElement.ParseValue(ref reader);
        switch (jsonElement)
        {
            case { ValueKind: JsonValueKind.Object }:
                return jsonElement.Deserialize<T>(options);
            case { ValueKind: JsonValueKind.Array } when jsonElement.GetArrayLength() > 0:
                throw new JsonException("Empty array is required");
            case { ValueKind: JsonValueKind.Array }:
            case { ValueKind: JsonValueKind.Null }:
                return default;
            default:
                throw new JsonException($"Unexpected token type {jsonElement.ValueKind}");
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
