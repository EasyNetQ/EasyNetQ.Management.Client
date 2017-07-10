using System.Collections.Generic;
using EasyNetQ.Management.Client.Model;
using NUnit.Framework;

namespace EasyNetQ.Management.Client.Tests.Model
{
    [TestFixture(Category = "Unit")]
    public class FederationSerializationTests
    {
        private List<Federation> federations;

        [SetUp]
        public void SetUp()
        {
            federations = ResourceLoader.LoadObjectFromJson<List<Federation>>("Federations.json");
        }

        [Test]
        public void Should_load_three_federation()
        {
            federations.Count.ShouldEqual(3);
        }

        [Test]
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