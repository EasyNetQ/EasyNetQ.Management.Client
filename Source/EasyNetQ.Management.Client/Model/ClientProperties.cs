using System;
using System.Collections.Generic;
using EasyNetQ.Management.Client.Dynamic;
using EasyNetQ.Management.Client.Serialization;
using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Model
{
        [JsonConverter(typeof(ClientPropertiesJsonConverter))]
        public class ClientProperties : PropertyExpando
        {
            public ClientProperties(IDictionary<String,Object> properties) : base (properties) {}

            public Capabilities Capabilities => GetPropertyOrDefault<Capabilities>("Capabilities");
            public string User => GetPropertyOrDefault<String>("User");
            public string Application => GetPropertyOrDefault<String>("Application");
            public string ClientApi => GetPropertyOrDefault<String>("ClientApi");
            public string ApplicationLocation => GetPropertyOrDefault<String>("ApplicationLocation");
            public DateTime Connected => GetPropertyOrDefault<DateTime>("Connected");
            public string EasynetqVersion => GetPropertyOrDefault<String>("EasynetqVersion");
            public string MachineName => GetPropertyOrDefault<String>("MachineName");
            public IDictionary<String, Object> PropertiesDictionary => Properties;
        }
}