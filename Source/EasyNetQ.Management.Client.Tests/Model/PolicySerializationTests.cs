namespace EasyNetQ.Management.Client.Tests.Model
{
    using System;
    using System.Linq;
    using Client.Model;
    using Newtonsoft.Json;
    using Xunit;

    public class PolicySerializationTests
    {
        private readonly Policy[] _policy;

        public PolicySerializationTests()
        {
            _policy = ResourceLoader.LoadObjectFromJson<Policy[]>("Policies_ha.json", ManagementClient.Settings);
        }

        [Fact]
        public void Should_read_apply_to_properly()
        {
            var exactlyPolicies = _policy.Where(p => p.Name == "ha-duplicate").ToList();
            Assert.Equal(1, exactlyPolicies.Count);
            var policy = exactlyPolicies.First();
            Assert.Equal(ApplyMode.Queues, policy.ApplyTo);

            exactlyPolicies = _policy.Where(p => p.Name == "mirror_test").ToList();
            Assert.Equal(1, exactlyPolicies.Count);
            policy = exactlyPolicies.First();
            Assert.Equal(ApplyMode.Exchanges, policy.ApplyTo);

            exactlyPolicies = _policy.Where(p => p.Name == "default apply to").ToList();
            Assert.Equal(1, exactlyPolicies.Count);
            policy = exactlyPolicies.First();
            Assert.Equal(ApplyMode.All, policy.ApplyTo);   
        }

           [Fact]
        public void Should_read_federation_upstream_properly()
        {
            var exactlyPolicies = _policy.Where(p => p.Name == "mirror_test").ToList();
            Assert.Equal(1, exactlyPolicies.Count);
            var policy = exactlyPolicies.First();
            Assert.Equal("test", policy.Definition.FederationUpstream);
        }

        [Fact]
        public void Should_read_exactly_ha_properly()
        {
            _policy.Count().ShouldEqual(3);
            var exactlyPolicies = _policy.Where(p => p.Name == "ha-duplicate");
            Assert.Equal(1, exactlyPolicies.Count());
            var policy = exactlyPolicies.First();
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
            _policy.Count().ShouldEqual(3);
            var mirrorTestPolicies = _policy.Where(p => p.Name == "mirror_test");
            Assert.Equal(1, mirrorTestPolicies.Count());
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
            }, ManagementClient.Settings);
            
            Assert.True(serializedMessage.Contains("\"apply-to\":\"all\""));

            serializedMessage = JsonConvert.SerializeObject(new Policy
            {
                Name = "bob",
                Pattern = "foo",
                ApplyTo = ApplyMode.Exchanges
            }, ManagementClient.Settings);

            Assert.True(serializedMessage.Contains("\"apply-to\":\"exchanges\""));

            serializedMessage = JsonConvert.SerializeObject(new Policy
            {
                Name = "bob",
                Pattern = "foo",
                ApplyTo = ApplyMode.Queues
            }, ManagementClient.Settings);
            Assert.True(serializedMessage.Contains("\"apply-to\":\"queues\""));
        }

        [Fact]
        public void Should_write_federation_upstream_properly()
        {
            var serializedMessage = JsonConvert.SerializeObject(new Policy
            {
                Name = "bob",
                Pattern = "foo",
                Definition = new PolicyDefinition { FederationUpstream = "my-upstream"}
            }, ManagementClient.Settings);

            Assert.True(serializedMessage.Contains("\"federation-upstream\":\"my-upstream\""));
        }

        [Fact]
        public void Should_write_all_ha_policy_without_param()
        {
            var serializedMessage = JsonConvert.SerializeObject(new Policy
            {
                Name = "bob",
                Pattern = "foo",
                Definition = new PolicyDefinition {HaMode = HaMode.All}
            }, ManagementClient.Settings);
            Console.WriteLine(serializedMessage);
            Assert.False(serializedMessage.Contains("ha-params"));
            Assert.False(serializedMessage.Contains("federation_upstream_set"));
        }

    }
}