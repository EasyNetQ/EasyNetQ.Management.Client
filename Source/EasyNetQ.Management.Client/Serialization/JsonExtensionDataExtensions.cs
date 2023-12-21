using System.Text.Json;

namespace EasyNetQ.Management.Client.Serialization;

public static class JsonExtensionDataExtensions
{
    public static IReadOnlyDictionary<string, object?>? ToExtensionData(IDictionary<string, JsonElement>? jsonExtensionData)
    {
        Dictionary<string, object?>? dictionary;
        if (jsonExtensionData == null)
        {
            dictionary = null;
        }
        else
        {
            dictionary = new Dictionary<string, object?>();
            foreach (var property in jsonExtensionData)
            {
                dictionary.Add(property.Key, property.Value.GetObjectValue());
            }
        }
        return dictionary;
    }
    public static IDictionary<string, JsonElement>? ToJsonExtensionData(IReadOnlyDictionary<string, object?>? extensionData)
    {
        IDictionary<string, JsonElement>? dictionary;
        if (extensionData == null)
        {
            dictionary = null;
        }
        else
        {
            dictionary = new Dictionary<string, JsonElement>();
            foreach (var property in extensionData)
            {
#if NET6_0_OR_GREATER
                dictionary.Add(property.Key, JsonSerializer.SerializeToElement(property.Value));
#else
                var bytes = JsonSerializer.SerializeToUtf8Bytes(property.Value);
                dictionary.Add(property.Key, JsonDocument.Parse(bytes).RootElement.Clone());
#endif
            }
        }
        return dictionary;
    }
}
