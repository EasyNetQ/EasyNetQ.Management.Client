using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Serialization;

internal sealed class SnakeCaseEnumConverter<TEnum>() : JsonStringEnumConverter<TEnum>(JsonNamingPolicy.SnakeCaseLower)
    where TEnum : struct, Enum;
