using System.Collections.ObjectModel;
using System.Text.Json;

namespace EasyNetQ.Management.Client.Serialization;

internal static class ObjectHelpers
{
    public static void WriteObjectValue(this Utf8JsonWriter writer, object? value)
    {
        switch (value)
        {
            case null:
                writer.WriteNullValue();
                return;
            case bool boolValue:
                writer.WriteBooleanValue(boolValue);
                return;
            case string stringValue:
                writer.WriteStringValue(stringValue);
                return;
            case long longValue:
                writer.WriteNumberValue(longValue);
                return;
            case int intValue:
                writer.WriteNumberValue(intValue);
                return;
            case IDictionary<string, object?> dictionaryValue:
                writer.WriteStartObject();
                foreach (var kvp in dictionaryValue)
                {
                    writer.WritePropertyName(kvp.Key);
                    WriteObjectValue(writer, kvp.Value);
                }

                writer.WriteEndObject();
                return;
            case IList<object?> listValue:
                writer.WriteStartArray();
                foreach (var item in listValue)
                    WriteObjectValue(writer, item);
                writer.WriteEndArray();
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static object? GetObjectValue(this in JsonElement jsonElement)
    {
        switch (jsonElement)
        {
            case { ValueKind: JsonValueKind.Array }:
            {
                var list = new List<object?>();
                foreach (var item in jsonElement.EnumerateArray())
                    list.Add(GetObjectValue(item));
                return new ReadOnlyCollection<object?>(list);
            }
            case { ValueKind: JsonValueKind.Object }:
            {
                var dictionary = new Dictionary<string, object?>();
                foreach (var property in jsonElement.EnumerateObject())
                    dictionary.Add(property.Name, GetObjectValue(property.Value));
                return new ReadOnlyDictionary<string, object?>(dictionary);
            }
            case { ValueKind: JsonValueKind.True }:
                return true;
            case { ValueKind: JsonValueKind.False }:
                return false;
            case { ValueKind: JsonValueKind.Number }:
                return jsonElement.TryGetInt64(out var longValue) ? longValue : jsonElement.GetDouble();
            case { ValueKind: JsonValueKind.String }:
            {
                var stringValue = jsonElement.GetString();
                if (DateTime.TryParse(stringValue, out var datetimeValue)) return datetimeValue;
                return stringValue;
            }
            case { ValueKind: JsonValueKind.Null }:
                return null;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
