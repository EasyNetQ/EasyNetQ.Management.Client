using System.Text.Json;
using System.Text.Json.Serialization;


namespace EasyNetQ.Management.Client.Serialization;

/// <summary>
/// Converts Unix time in ms to DateTime and vice versa - based on
/// http://stackapps.com/questions/1175/how-to-convert-unix-timestamp-to-net-datetime/1176#1176
/// </summary>
public class UnixMsDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetInt64().FromUnixTimeMs();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeMs());
    }
}
