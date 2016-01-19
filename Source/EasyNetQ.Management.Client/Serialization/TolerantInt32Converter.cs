using System;
using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Serialization
{
    public class TolerantInt32Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.TokenType == JsonToken.Integer ? Convert.ToInt32(reader.Value) : default(int);
        }
    }
}