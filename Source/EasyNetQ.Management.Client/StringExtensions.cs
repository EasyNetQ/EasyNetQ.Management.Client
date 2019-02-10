using System.Security;

namespace EasyNetQ.Management.Client
{
    internal static class StringExtensions
    {
        public static SecureString Secure(this string input)
        {
            var result = new SecureString();
            foreach (var c in input)
                result.AppendChar(c);
            result.MakeReadOnly();
            return result;
        }
    }
}