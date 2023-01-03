using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Context(
    string Node,
    string Description,
    string Path,
    [property: JsonConverter(typeof(TolerantInt32Converter))]
    int Port
);
