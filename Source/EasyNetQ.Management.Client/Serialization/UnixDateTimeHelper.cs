namespace EasyNetQ.Management.Client.Serialization;

///<summary>
/// http://stackapps.com/questions/1175/how-to-convert-unix-timestamp-to-net-datetime/1176#1176
///</summary>
internal static class UnixDateTimeHelper
{
    private const string InvalidUnixEpochErrorMessage = "Unix epoc starts January 1st, 1970";

    public static DateTime FromUnixTimeMs(this long self)
    {
        var ret = new DateTime(1970, 1, 1);
        return ret.AddMilliseconds(self);
    }

    public static long ToUnixTimeMs(this DateTime self)
    {
        if (self == DateTime.MinValue)
        {
            return 0;
        }

        var epoc = new DateTime(1970, 1, 1);
        var delta = self - epoc;

        if (delta.TotalSeconds < 0) throw new ArgumentOutOfRangeException(InvalidUnixEpochErrorMessage);

        return (long)delta.TotalMilliseconds;
    }
}
