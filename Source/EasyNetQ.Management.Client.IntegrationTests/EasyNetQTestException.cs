using System;

namespace EasyNetQ.Management.Client.IntegrationTests;

/// <summary>
///     Exception thrown when something unexpected happens in tests
/// </summary>
public class EasyNetQTestException : Exception
{
    public EasyNetQTestException(string message)
        : base(message)
    {
    }

    public EasyNetQTestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
