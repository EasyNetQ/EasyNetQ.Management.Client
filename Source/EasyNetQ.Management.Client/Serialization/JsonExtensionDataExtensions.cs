using System.Text.Json;

namespace EasyNetQ.Management.Client.Serialization;

internal static class JsonExtensionDataExtensions
{
    public static IReadOnlyDictionary<string, object?>? ToExtensionData(IDictionary<string, JsonElement>? jsonExtensionData)
    {
        if (jsonExtensionData == null) return null;

        var extensionData = new Dictionary<string, object?>(jsonExtensionData.Count);
        foreach (var property in jsonExtensionData)
            extensionData.Add(property.Key, property.Value.GetObjectValue());
        return extensionData;
    }

    public static IDictionary<string, JsonElement>? ToJsonExtensionData(IReadOnlyDictionary<string, object?>? extensionData)
    {
        if (extensionData == null) return null;

        var jsonExtensionData = new Dictionary<string, JsonElement>(extensionData.Count);
        foreach (var property in extensionData)
        {
            jsonExtensionData.Add(property.Key, JsonSerializer.SerializeToElement(property.Value));
        }
        return jsonExtensionData;
    }
}
