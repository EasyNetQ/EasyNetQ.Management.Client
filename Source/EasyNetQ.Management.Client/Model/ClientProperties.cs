using EasyNetQ.Management.Client.Dynamic;
using EasyNetQ.Management.Client.Serialization;
using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Model;

[JsonConverter(typeof(ClientPropertiesJsonConverter))]
public class ClientProperties : PropertyExpando
{
    public ClientProperties(IDictionary<string, object> properties) : base(properties) { }

    public Capabilities Capabilities => GetPropertyOrDefault<Capabilities>("Capabilities");
    public string User => GetPropertyOrDefault<string>("User");
    public string Application => GetPropertyOrDefault<string>("Application");
    public string ClientApi => GetPropertyOrDefault<string>("ClientApi");
    public string ApplicationLocation => GetPropertyOrDefault<string>("ApplicationLocation");
    public DateTime Connected => GetPropertyOrDefault<DateTime>("Connected");
    public string EasynetqVersion => GetPropertyOrDefault<string>("EasynetqVersion");
    public string MachineName => GetPropertyOrDefault<string>("MachineName");
    public IDictionary<string, object> PropertiesDictionary => Properties;
}
