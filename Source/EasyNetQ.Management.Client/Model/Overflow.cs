namespace EasyNetQ.Management.Client.Model;

public enum Overflow
{
    [EnumMember(Value = "drop-head")]
    DropHead,
    [EnumMember(Value = "reject-publish")]
    RejectPublish,
    [EnumMember(Value = "reject-publish-dlx")]
    RejectPublishDlx
}
