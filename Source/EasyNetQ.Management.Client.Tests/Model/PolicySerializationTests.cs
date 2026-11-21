using System.Text.Json;
using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.Tests.Model;

public class PolicySerializationTests
{
    private readonly Policy[] policies;

    public PolicySerializationTests()
    {
        policies = ResourceLoader.LoadObjectFromJson<Policy[]>("Policies.json", ManagementClient.SerializerOptions);
    }

    [Fact]
    public void Should_read_apply_to_properly()
    {
        var exactlyPolicies = policies.Where(p => p.Name == "quorum").ToList();
        Assert.Single(exactlyPolicies);
        var policy = exactlyPolicies.First();
        Assert.Equal(ApplyMode.Queues, policy.ApplyTo);

        exactlyPolicies = policies.Where(p => p.Name == "federated").ToList();
        Assert.Single(exactlyPolicies);
        policy = exactlyPolicies.First();
        Assert.Equal(ApplyMode.Exchanges, policy.ApplyTo);

        exactlyPolicies = policies.Where(p => p.Name == "default apply to").ToList();
        Assert.Single(exactlyPolicies);
        policy = exactlyPolicies.First();
        Assert.Equal(ApplyMode.All, policy.ApplyTo);
    }

    [Fact]
    public void Should_read_federation_upstream_properly()
    {
        var exactlyPolicies = policies.Where(p => p.Name == "federated").ToList();
        Assert.Single(exactlyPolicies);
        var policy = exactlyPolicies.First();
        Assert.Equal("test", policy.Definition.FederationUpstream);
    }

    [Fact]
    public void Should_read_quorum_policy_properly()
    {
        policies.Length.Should().Be(3);
        var policy = policies.Single(p => p.Name == "quorum");
        Assert.Equal("^q.*", policy.Pattern);
        Assert.Equal(1, policy.Priority);
        Assert.Equal(5u, policy.Definition.DeliveryLimit);
        Assert.Equal(DeadLetterStrategy.AtLeastOnce, policy.Definition.DeadLetterStrategy);
        Assert.Equal(QueueLocator.Balanced, policy.Definition.QueueLeaderLocator);
        Assert.Equal(Overflow.RejectPublish, policy.Definition.Overflow);
        Assert.Equal("dlx", policy.Definition.DeadLetterExchange);
    }

    [Fact]
    public void Should_write_apply_to_properly()
    {
        var serializedMessage = JsonSerializer.Serialize(
            new Policy(
                Name: "bob",
                Pattern: "foo",
                Definition: new PolicyDefinition()
            ),
            ManagementClient.SerializerOptions
        );

        Assert.Contains("\"apply-to\":\"all\"", serializedMessage);

        serializedMessage = JsonSerializer.Serialize(
            new Policy(
                Name: "bob",
                Pattern: "foo",
                Definition: new PolicyDefinition(),
                ApplyTo: ApplyMode.Exchanges
            ),
            ManagementClient.SerializerOptions
        );

        Assert.Contains("\"apply-to\":\"exchanges\"", serializedMessage);

        serializedMessage = JsonSerializer.Serialize(
            new Policy(
                Name: "bob",
                Pattern: "foo",
                Definition: new PolicyDefinition(),
                ApplyTo: ApplyMode.Queues
            ),
            ManagementClient.SerializerOptions
        );
        Assert.Contains("\"apply-to\":\"queues\"", serializedMessage);
    }

    [Fact]
    public void Should_write_federation_upstream_properly()
    {
        var serializedMessage = JsonSerializer.Serialize(
            new Policy(
                Name: "bob",
                Pattern: "foo",
                Definition: new PolicyDefinition { FederationUpstream = "my-upstream" }
            ),
            ManagementClient.SerializerOptions
        );

        Assert.Contains("\"federation-upstream\":\"my-upstream\"", serializedMessage);
    }

    [Fact]
    public void Should_write_quorum_policy_with_dashed_enum_values()
    {
        var serializedMessage = JsonSerializer.Serialize(
            new Policy(
                Name: "bob",
                Pattern: "foo",
                Definition: new PolicyDefinition(DeadLetterStrategy: DeadLetterStrategy.AtLeastOnce, Overflow: Overflow.RejectPublish)
            ),
            ManagementClient.SerializerOptions
        );

        Assert.Contains("\"dead-letter-strategy\":\"at-least-once\"", serializedMessage);
        Assert.Contains("\"overflow\":\"reject-publish\"", serializedMessage);
        Assert.DoesNotContain("federation-upstream-set", serializedMessage);
    }
}
