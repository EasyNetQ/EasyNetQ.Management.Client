using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.Tests.Model;

public class FederationSerializationTests
{
    private readonly List<Federation> federations;

    public FederationSerializationTests()
    {
        federations = ResourceLoader.LoadObjectFromJson<List<Federation>>("Federations.json", ManagementClient.SerializerOptions);
    }

    [Fact]
    public void Should_load_three_federation()
    {
        federations.Count.Should().Be(3);
    }

    [Fact]
    public void Should_have_federation_properties()
    {
        var federation = federations[0];

        federation.Status.Should().Be(FederationStatus.Running);
        federation.Node.Should().Be("rabbit@oms-rabbitmq-03");
        federation.Type.Should().Be("exchange");
    }
}
