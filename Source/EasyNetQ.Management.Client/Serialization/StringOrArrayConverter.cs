using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace EasyNetQ.Management.Client.Serialization;

public class StringOrArrayConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(string);
    }

    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer
    )
    {
        var token = JToken.Load(reader);
        return token.Type == JTokenType.Array
            ? string.Join(",", token.ToObject<string[]>())
            : token.ToString();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
