using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Serialization;

internal sealed class ObjectToStringConverter<T> : JsonConverter<T>
{
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString());
    }

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
