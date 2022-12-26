using EasyNetQ.Management.Client.Model;
using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Tests.Model;

public class PolicySerializationTests
{
    private readonly Policy[] policies;

    public PolicySerializationTests()
    {
        policies = ResourceLoader.LoadObjectFromJson<Policy[]>("Policies_ha.json", ManagementClient.SerializerSettings);
    }

    [Fact]
    public void Should_read_apply_to_properly()
    {
        var exactlyPolicies = policies.Where(p => p.Name == "ha-duplicate").ToList();
        Assert.Single(exactlyPolicies);
        var policy = exactlyPolicies.First();
        Assert.Equal(ApplyMode.Queues, policy.ApplyTo);

        exactlyPolicies = policies.Where(p => p.Name == "mirror_test").ToList();
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
        var exactlyPolicies = policies.Where(p => p.Name == "mirror_test").ToList();
        Assert.Single(exactlyPolicies);
        var policy = exactlyPolicies.First();
        Assert.Equal("test", policy.Definition.FederationUpstream);
    }

    [Fact]
    public void Should_read_exactly_ha_properly()
    {
        policies.Length.Should().Be(3);
        var exactlyPolicies = policies.Where(p => p.Name == "ha-duplicate").ToList();
        Assert.Single(exactlyPolicies);
        var policy = exactlyPolicies[0];
        Assert.Equal(HaMode.Exactly, policy.Definition.HaMode);
        Assert.Equal(HaMode.Exactly, policy.Definition.HaParams.AssociatedHaMode);
        Assert.Equal(2, policy.Definition.HaParams.ExactlyCount);
        Assert.Equal("^dup.*", policy.Pattern);
        Assert.Equal(HaSyncMode.Manual, policy.Definition.HaSyncMode);
        Assert.Equal(1, policy.Priority);
    }

    [Fact]
    public void Should_read_nodes_ha_properly()
    {
        policies.Length.Should().Be(3);
        var mirrorTestPolicies = policies.Where(p => p.Name == "mirror_test").ToList();
        Assert.Single(mirrorTestPolicies);
        var policy = mirrorTestPolicies.First();
        Assert.Equal(HaMode.Nodes, policy.Definition.HaMode);
        Assert.Equal(HaMode.Nodes, policy.Definition.HaParams.AssociatedHaMode);
        Assert.Equal(new[] { "rabbit@rab5", "rabbit@rab6" }, policy.Definition.HaParams.Nodes);
        Assert.Equal("mirror", policy.Pattern);
        Assert.Equal(HaSyncMode.Automatic, policy.Definition.HaSyncMode);
        Assert.Equal(0, policy.Priority);
    }

    [Fact]
    public void Should_write_apply_to_properly()
    {
        var serializedMessage = JsonConvert.SerializeObject(new Policy
        {
            Name = "bob",
            Pattern = "foo"
        }, ManagementClient.SerializerSettings);

        Assert.Contains("\"apply-to\":\"all\"", serializedMessage);

        serializedMessage = JsonConvert.SerializeObject(new Policy
        {
            Name = "bob",
            Pattern = "foo",
            ApplyTo = ApplyMode.Exchanges
        }, ManagementClient.SerializerSettings);

        Assert.Contains("\"apply-to\":\"exchanges\"", serializedMessage);

        serializedMessage = JsonConvert.SerializeObject(new Policy
        {
            Name = "bob",
            Pattern = "foo",
            ApplyTo = ApplyMode.Queues
        }, ManagementClient.SerializerSettings);
        Assert.Contains("\"apply-to\":\"queues\"", serializedMessage);
    }

    [Fact]
    public void Should_write_federation_upstream_properly()
    {
        var serializedMessage = JsonConvert.SerializeObject(new Policy
        {
            Name = "bob",
            Pattern = "foo",
            Definition = new PolicyDefinition { FederationUpstream = "my-upstream" }
        }, ManagementClient.SerializerSettings);

        Assert.Contains("\"federation-upstream\":\"my-upstream\"", serializedMessage);
    }

    [Fact]
    public void Should_write_all_ha_policy_without_param()
    {
        var serializedMessage = JsonConvert.SerializeObject(new Policy
        {
            Name = "bob",
            Pattern = "foo",
            Definition = new PolicyDefinition { HaMode = HaMode.All }
        }, ManagementClient.SerializerSettings);

        Assert.DoesNotContain("ha-params", serializedMessage);
        Assert.DoesNotContain("ha-sync-batch-size", serializedMessage);
        Assert.DoesNotContain("federation-upstream-set", serializedMessage);
    }
}
