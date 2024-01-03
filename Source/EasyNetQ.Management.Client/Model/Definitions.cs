using EasyNetQ.Management.Client.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace EasyNetQ.Management.Client.Model;

public record Definitions(
    string RabbitVersion,
    string RabbitmqVersion,
    string ProductName,
    string ProductVersion,
    IReadOnlyList<User> Users,
    IReadOnlyList<Vhost> Vhosts,
    IReadOnlyList<Permission> Permissions,
    IReadOnlyList<TopicPermission> TopicPermissions,
    IReadOnlyList<Parameter> Parameters,
    IReadOnlyList<GlobalParameter> GlobalParameters,
    IReadOnlyList<Policy> Policies,
    IReadOnlyList<Queue> Queues,
    IReadOnlyList<Exchange> Exchanges,
    IReadOnlyList<Binding> Bindings
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
