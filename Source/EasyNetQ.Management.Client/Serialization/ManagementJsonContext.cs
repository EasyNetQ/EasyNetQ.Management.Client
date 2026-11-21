using System.Text.Json;
using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = [
        typeof(SnakeCaseEnumConverter<AckMode>),
        typeof(SnakeCaseEnumConverter<ApplyMode>),
        typeof(SnakeCaseEnumConverter<DeadLetterStrategy>),
        typeof(SnakeCaseEnumConverter<FederationStatus>),
        typeof(SnakeCaseEnumConverter<Overflow>),
        typeof(SnakeCaseEnumConverter<PayloadEncoding>),
        typeof(SnakeCaseEnumConverter<QueueLocator>),
        typeof(SnakeCaseEnumConverter<QueueType>),
    ])]
[JsonSerializable(typeof(Overview))]
[JsonSerializable(typeof(Definitions))]
[JsonSerializable(typeof(IReadOnlyList<Node>))]
[JsonSerializable(typeof(IReadOnlyList<Connection>))]
[JsonSerializable(typeof(Channel))]
[JsonSerializable(typeof(IReadOnlyList<Channel>))]
[JsonSerializable(typeof(Exchange))]
[JsonSerializable(typeof(IReadOnlyList<Exchange>))]
[JsonSerializable(typeof(PageResult<Exchange>))]
[JsonSerializable(typeof(ExchangeInfo))]
[JsonSerializable(typeof(PublishInfo))]
[JsonSerializable(typeof(PublishResult))]
[JsonSerializable(typeof(Queue))]
[JsonSerializable(typeof(IReadOnlyList<Queue>))]
[JsonSerializable(typeof(PageResult<Queue>))]
[JsonSerializable(typeof(QueueInfo))]
[JsonSerializable(typeof(GetMessagesFromQueueInfo))]
[JsonSerializable(typeof(IReadOnlyList<Message>))]
[JsonSerializable(typeof(IReadOnlyList<Binding>))]
[JsonSerializable(typeof(BindingInfo))]
[JsonSerializable(typeof(Vhost))]
[JsonSerializable(typeof(IReadOnlyList<Vhost>))]
[JsonSerializable(typeof(VhostInfo))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(IReadOnlyList<User>))]
[JsonSerializable(typeof(UserInfo))]
[JsonSerializable(typeof(IReadOnlyList<Permission>))]
[JsonSerializable(typeof(PermissionInfo))]
[JsonSerializable(typeof(IReadOnlyList<TopicPermission>))]
[JsonSerializable(typeof(TopicPermissionInfo))]
[JsonSerializable(typeof(IReadOnlyList<Consumer>))]
[JsonSerializable(typeof(IReadOnlyList<Policy>))]
[JsonSerializable(typeof(PolicyInfo))]
[JsonSerializable(typeof(Parameter))]
[JsonSerializable(typeof(IReadOnlyList<Parameter>))]
[JsonSerializable(typeof(ParameterValue))]
[JsonSerializable(typeof(ParameterShovelValue))]
[JsonSerializable(typeof(ParameterFederationValue))]
[JsonSerializable(typeof(IReadOnlyList<Federation>))]
[JsonSerializable(typeof(IReadOnlyList<ShovelStatus>))]
[JsonSerializable(typeof(IReadOnlyList<UserLimits>))]
[JsonSerializable(typeof(LimitInfo))]
[JsonSerializable(typeof(ClusterName))]
[JsonSerializable(typeof(Node))]
[JsonSerializable(typeof(NodeMemory))]
[JsonSerializable(typeof(VhostDefinitions))]
[JsonSerializable(typeof(IReadOnlyList<FeatureFlag>))]
[JsonSerializable(typeof(IReadOnlyList<DeprecatedFeature>))]
[JsonSerializable(typeof(Connection))]
[JsonSerializable(typeof(IReadOnlyList<VhostLimits>))]
[JsonSerializable(typeof(CurrentUser))]
[JsonSerializable(typeof(UserNames))]
[JsonSerializable(typeof(Permission))]
[JsonSerializable(typeof(Policy))]
[JsonSerializable(typeof(GlobalParameter))]
[JsonSerializable(typeof(IReadOnlyList<GlobalParameter>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(JsonElement))]
internal sealed partial class ManagementJsonContext : JsonSerializerContext;
