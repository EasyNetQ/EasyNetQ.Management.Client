#if !NET8_0_OR_GREATER && !NETSTANDARD2_0

namespace System.Text.Json.Serialization;

internal sealed class JsonNumberEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private static readonly TypeCode s_enumTypeCode = Type.GetTypeCode(typeof(TEnum));
    static JsonNumberEnumConverter()
    {
        switch (s_enumTypeCode)
        {
            case TypeCode.Int32: break;
            default: throw new NotImplementedException($"{typeof(JsonNumberEnumConverter<TEnum>)}: underlying type {s_enumTypeCode} is not supported");
        }
    }

    public JsonNumberEnumConverter() { }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (s_enumTypeCode)
        {
            case TypeCode.Int32: return (TEnum)(object)reader.GetInt32();
            default: throw new NotImplementedException();
        }
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        switch (s_enumTypeCode)
        {
            case TypeCode.Int32: writer.WriteNumberValue((int)(object)value); break;
            default: throw new NotImplementedException();
        }
    }
}

#endif
