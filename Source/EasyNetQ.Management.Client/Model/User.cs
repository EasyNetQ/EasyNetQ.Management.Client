using EasyNetQ.Management.Client.Serialization;
using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Model;

public class User
{
    public string Name { get; set; }
    public string PasswordHash { get; set; }
    public List<UserTag> Tags { get; set; }
}
