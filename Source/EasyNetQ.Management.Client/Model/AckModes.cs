namespace EasyNetQ.Management.Client.Model;

public enum AckModes
{
    AckRequeueFalse,
    AckRequeueTrue,
    RejectRequeueFalse,
    RejectRequeueTrue
}

internal static class AckModeExtensions
{
    public static string ToSnakeCaseString(this AckModes ackModes)
    {
        return ackModes switch
        {
            AckModes.AckRequeueFalse => "ack_requeue_false",
            AckModes.AckRequeueTrue => "ack_requeue_true",
            AckModes.RejectRequeueFalse => "reject_requeue_false",
            AckModes.RejectRequeueTrue => "reject_requeue_true",
            _ => throw new ArgumentOutOfRangeException(nameof(ackModes), ackModes, null)
        };
    }
}
