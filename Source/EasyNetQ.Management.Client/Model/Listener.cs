using EasyNetQ.Management.Client.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Listener(
    string Node,
    string Protocol,
    string IpAddress,
    int Port,
    [property: JsonConverter(typeof(EmptyArrayAsDefaultConverter<SocketOpts>))]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    SocketOpts? SocketOpts = null
)
{
    [JsonExtensionData()]
    public IDictionary<string, JsonElement>? JsonExtensionData { get; set; }

    [JsonIgnore()]
    public IReadOnlyDictionary<string, object?>? ExtensionData
    {
        get { return JsonExtensionDataExtensions.ToExtensionData(JsonExtensionData); }
        set { JsonExtensionData = JsonExtensionDataExtensions.ToJsonExtensionData(value); }
    }
};
