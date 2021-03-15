// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model
{
    public class ExchangeInfoTests
    {
        [Fact]
        public void Should_be_able_to_get_name()
        {
            const string expectedName = "the_name";
            var exchangeInfo = new ExchangeInfo(expectedName, "direct");
            exchangeInfo.GetName().Should().Be(expectedName);
        }

        [Fact]
        public void Should_throw_if_we_try_to_create_an_exchange_with_an_unknown_type()
        {
            Assert.Throws<EasyNetQManagementException>(() => new ExchangeInfo("management_api_test_exchange", "some_bullshit_exchange_type"));
        }
    }
}

// ReSharper restore InconsistentNaming
