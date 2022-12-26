// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.Tests.Model;

public class OverviewSerializationTests
{
    private readonly Overview overview;

    public OverviewSerializationTests()
    {
        overview = ResourceLoader.LoadObjectFromJson<Overview>("Overview.json", ManagementClient.SerializerSettings);
    }

    [Fact]
    public void Should_contain_management_version()
    {
        overview.ManagementVersion.Should().Be("2.8.6");
        overview.StatisticsLevel.Should().Be("fine");
    }

    [Fact]
    public void Should_contain_exchange_types()
    {
        overview.ExchangeTypes[0].Name.Should().Be("topic");
        overview.ExchangeTypes[0].Description.Should().Be("AMQP topic exchange, as per the AMQP specification");
        overview.ExchangeTypes[0].Enabled.Should().BeTrue();
    }
}

// ReSharper restore InconsistentNaming
