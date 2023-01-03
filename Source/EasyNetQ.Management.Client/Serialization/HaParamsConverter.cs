using System.Text.Json;
using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.Serialization;

internal class HaParamsConverter : JsonConverter<HaParams>
{
    // Support serializing/deserializing ha-params according to http://www.rabbitmq.com/ha.html#genesis for 3.1.3
    public override HaParams Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return new HaParams(AssociatedHaMode: HaMode.Exactly, ExactlyCount: reader.GetInt64());

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            reader.Read();

            var nodes = new List<string>();

            while (reader.TokenType == JsonTokenType.String)
            {
                var node = reader.GetString();
                if (node != null)
                    nodes.Add(node);
                reader.Read();
            }

            return new HaParams(AssociatedHaMode: HaMode.Nodes, Nodes: nodes);
        }

        throw new JsonException("Failed to deserialize");
    }

    public override void Write(Utf8JsonWriter writer, HaParams value, JsonSerializerOptions options)
    {
        if (value is { AssociatedHaMode: HaMode.Exactly })
        {
            writer.WriteNumberValue(value.ExactlyCount ?? 0);
        }
        else if (value is { AssociatedHaMode: HaMode.Nodes })
        {
            writer.WriteStartArray();
            foreach (var node in value.Nodes ?? Array.Empty<string>())
                writer.WriteStringValue(node);
            writer.WriteEndArray();
        }
    }
}
