using System.Collections.Generic;
using EasyNetQ.Management.Client.Model;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model
{
    public class FederationSerializationTests
    {
        private readonly List<Federation> federations;

        public FederationSerializationTests()
        {
            federations = ResourceLoader.LoadObjectFromJson<List<Federation>>("Federations.json");
        }

        [Fact]
        public void Should_load_three_federation()
        {
            federations.Count.ShouldEqual(3);
        }

        [Fact]
        public void Should_have_federation_properties()
        {
            var federation = federations[0];

            federation.Status.ShouldEqual(FederationStatus.Running);
            federation.Node.ShouldEqual("rabbit@oms-rabbitmq-03");
            federation.Type.ShouldEqual("exchange");
        }
    }
}

// ReSharper restore InconsistentNaming