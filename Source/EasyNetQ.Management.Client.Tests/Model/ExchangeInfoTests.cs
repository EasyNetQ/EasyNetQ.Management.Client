// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model;

public class ExchangeInfoTests
{
    [Fact]
    public void Should_be_able_to_get_name()
    {
        const string expectedName = "the_name";
        var exchangeInfo = new ExchangeInfo(expectedName, "direct");
        exchangeInfo.Name.Should().Be(expectedName);
    }
}

// ReSharper restore InconsistentNaming
