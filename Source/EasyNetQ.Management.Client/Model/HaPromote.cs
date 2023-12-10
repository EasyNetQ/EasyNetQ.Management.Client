using System.Runtime.Serialization;

namespace EasyNetQ.Management.Client.Model;

public enum HaPromote
{
    [EnumMember(Value = "when-synced")]
    WhenSynced,
    [EnumMember(Value = "always")]
    Always
}
