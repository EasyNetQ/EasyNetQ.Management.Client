using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Serialization;

internal class DoubleNamedFloatingPointLiteralsConverter : JsonConverter<double>
{
    private readonly string PositiveInfinity = "Infinity";
    private readonly string NegativeInfinity = "-Infinity";
    private readonly string NaN = "NaN";

    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? stringValue = reader.GetString();
            if (string.Equals(stringValue, PositiveInfinity, StringComparison.OrdinalIgnoreCase))
            {
                return double.PositiveInfinity;
            }
            else if (string.Equals(stringValue, NegativeInfinity, StringComparison.OrdinalIgnoreCase))
            {
                return double.NegativeInfinity;
            }
            else if (string.Equals(stringValue, NaN, StringComparison.OrdinalIgnoreCase))
            {
                return double.NaN;
            }
            else
            {
                throw new JsonException($"Expected floating point literal ({PositiveInfinity}, {NegativeInfinity} or {NaN}), but was {stringValue}");
            }
        }

        return reader.GetDouble();
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case double.PositiveInfinity: writer.WriteStringValue(PositiveInfinity); break;
            case double.NegativeInfinity: writer.WriteStringValue(NegativeInfinity); break;
            case double.NaN: writer.WriteStringValue(NaN); break;
            default: writer.WriteNumberValue(value); break;
        }
    }
}
