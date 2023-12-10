using System.Runtime.Serialization;

namespace EasyNetQ.Management.Client.Model;

public enum QueueLocator
{
    [EnumMember(Value = "client-local")]
    ClientLocal,
    [EnumMember(Value = "balanced")]
    Balanced
}
