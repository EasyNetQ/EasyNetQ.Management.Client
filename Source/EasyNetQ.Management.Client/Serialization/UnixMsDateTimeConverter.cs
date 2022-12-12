using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EasyNetQ.Management.Client.Serialization;

/// <summary>
/// Converts Unix time in ms to DateTime and vice versa - based on
/// http://stackapps.com/questions/1175/how-to-convert-unix-timestamp-to-net-datetime/1176#1176
/// </summary>
public class UnixMsDateTimeConverter : DateTimeConverterBase
{
    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is DateTime dateTime)
        {
            writer.WriteValue(dateTime.ToUnixTimeMs());
        }
        else
        {
            throw new Exception("Expected date object value.");
        }
    }

    /// <summary>
    ///   Reads the JSON representation of the object.
    /// </summary>
    /// <param name = "reader">The <see cref = "JsonReader" /> to read from.</param>
    /// <param name = "objectType">Type of the object.</param>
    /// <param name = "existingValue">The existing value of object being read.</param>
    /// <param name = "serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override object ReadJson(
        JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer
    )
    {
        if (reader.TokenType != JsonToken.Integer)
            throw new Exception("Wrong Token Type");

        var ticks = (long)reader.Value!;
        return ticks.FromUnixTimeMs();
    }
}
