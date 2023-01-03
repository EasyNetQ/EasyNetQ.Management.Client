using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Serialization;

internal class TolerantInt32Converter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetInt32();
        if (reader.TokenType == JsonTokenType.String && int.TryParse(reader.GetString(), out var intValue))
            return intValue;
        return default;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
