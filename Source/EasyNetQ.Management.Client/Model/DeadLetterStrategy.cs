using System.Runtime.Serialization;

namespace EasyNetQ.Management.Client.Model;

public enum DeadLetterStrategy
{
    [EnumMember(Value = "at-most-once")]
    AtMostOnce,
    [EnumMember(Value = "at-least-once")]
    AtLeastOnce
}
