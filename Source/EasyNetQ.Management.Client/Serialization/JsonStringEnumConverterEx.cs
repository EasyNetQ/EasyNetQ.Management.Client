#if NET
using System.Diagnostics.CodeAnalysis;
#endif
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace EasyNetQ.Management.Client.Serialization;

// https://github.com/dotnet/runtime/issues/31081#issuecomment-848697673
internal class JsonStringEnumConverterEx<
#if NET
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
#endif
    TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private readonly Dictionary<TEnum, string> _enumToString = new Dictionary<TEnum, string>();
    private readonly Dictionary<string, TEnum> _stringToEnum = new Dictionary<string, TEnum>();

    public JsonStringEnumConverterEx()
    {
        foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var value = (TEnum)field.GetValue(null)!;
            var name = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? field.Name;
            _enumToString.Add(value, name);
            _stringToEnum.Add(name, value);
        }
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue != null && _stringToEnum.TryGetValue(stringValue, out var enumValue))
        {
            return enumValue;
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(_enumToString[value]);
    }
}
