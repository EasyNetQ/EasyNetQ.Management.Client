using EasyNetQ.Management.Client.Serialization;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record ParameterFederationValue(
    [property: JsonConverter(typeof(ObjectToStringConverter<AmqpUri>))]
    AmqpUri Uri,
    int Expires
);
