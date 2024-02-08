using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.Tests.Model;

public class ExchangeInfoTests
{
    [Fact]
    public void Should_be_able_to_get_type()
    {
        const string expectedType = "direct";
        var exchangeInfo = new ExchangeInfo("direct");
        exchangeInfo.Type.Should().Be(expectedType);
    }
}
