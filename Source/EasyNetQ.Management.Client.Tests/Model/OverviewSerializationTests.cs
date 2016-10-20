// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model
{
    public class OverviewSerializationTests
    {
        private readonly Overview overview;

        public OverviewSerializationTests()
        {
            overview = ResourceLoader.LoadObjectFromJson<Overview>("Overview.json", ManagementClient.Settings);
        }

        [Fact]
        public void Should_contain_management_version()
        {
            overview.ManagementVersion.ShouldEqual("2.8.6");
            overview.StatisticsLevel.ShouldEqual("fine");
        }

        [Fact]
        public void Should_congtain_exchange_types()
        {
            overview.ExchangeTypes[0].Name.ShouldEqual("topic");
            overview.ExchangeTypes[0].Description.ShouldEqual("AMQP topic exchange, as per the AMQP specification");
            overview.ExchangeTypes[0].Enabled.ShouldBeTrue();
        }
    }
}

// ReSharper restore InconsistentNaming