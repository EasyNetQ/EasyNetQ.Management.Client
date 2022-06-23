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
        JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.Array)
            return string.Join(",", token.ToObject<string[]>());
        else
            return token.ToString();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
