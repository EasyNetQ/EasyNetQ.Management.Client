using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record LengthsSample(
    long Sample,
    [property: JsonConverter(typeof(UnixMsDateTimeConverter))] DateTime Timestamp
);
