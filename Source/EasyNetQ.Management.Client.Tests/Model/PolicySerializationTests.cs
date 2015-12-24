namespace EasyNetQ.Management.Client.Tests.Model
{
    using System;
    using System.Linq;
    using Client.Model;
    using Newtonsoft.Json;
    using NUnit.Framework;

    [TestFixture(Category = "Unit")]
    public class PolicySerializationTests
    {
        private Policy[] _policy;

        [SetUp]
        public void SetUp()
        {
            _policy = ResourceLoader.LoadObjectFromJson<Policy[]>("Policies_ha.json", ManagementClient.Settings);
        }

        [Test]
        public void Should_read_apply_to_properly()
        {
            var exactlyPolicies = _policy.Where(p => p.Name == "ha-duplicate").ToList();
            Assert.AreEqual(1, exactlyPolicies.Count);
            var policy = exactlyPolicies.First();
            Assert.AreEqual(ApplyMode.Queues, policy.ApplyTo);

            exactlyPolicies = _policy.Where(p => p.Name == "mirror_test").ToList();
            Assert.AreEqual(1, exactlyPolicies.Count);
            policy = exactlyPolicies.First();
            Assert.AreEqual(ApplyMode.Exchanges, policy.ApplyTo);

            exactlyPolicies = _policy.Where(p => p.Name == "default apply to").ToList();
            Assert.AreEqual(1, exactlyPolicies.Count);
            policy = exactlyPolicies.First();
            Assert.AreEqual(ApplyMode.All, policy.ApplyTo);   
        }

           [Test]
        public void Should_read_federation_upstream_properly()
        {
            var exactlyPolicies = _policy.Where(p => p.Name == "mirror_test").ToList();
            Assert.AreEqual(1, exactlyPolicies.Count);
            var policy = exactlyPolicies.First();
            Assert.AreEqual("test", policy.Definition.FederationUpstream);
        }

        [Test]
        public void Should_read_exactly_ha_properly()
        {
            _policy.Count().ShouldEqual(3);
            var exactlyPolicies = _policy.Where(p => p.Name == "ha-duplicate");
            Assert.AreEqual(1, exactlyPolicies.Count());
            var policy = exactlyPolicies.First();
            Assert.AreEqual(HaMode.Exactly, policy.Definition.HaMode);
            Assert.AreEqual(HaMode.Exactly, policy.Definition.HaParams.AssociatedHaMode);
            Assert.AreEqual(2, policy.Definition.HaParams.ExactlyCount);
            Assert.AreEqual("^dup.*", policy.Pattern);
            Assert.AreEqual(HaSyncMode.Manual, policy.Definition.HaSyncMode);
            Assert.AreEqual(1, policy.Priority);
        }

        [Test]
        public void Should_read_nodes_ha_properly()
        {
            _policy.Count().ShouldEqual(3);
            var mirrorTestPolicies = _policy.Where(p => p.Name == "mirror_test");
            Assert.AreEqual(1, mirrorTestPolicies.Count());
            var policy = mirrorTestPolicies.First();
            Assert.AreEqual(HaMode.Nodes, policy.Definition.HaMode);
            Assert.AreEqual(HaMode.Nodes, policy.Definition.HaParams.AssociatedHaMode);
            Assert.AreEqual(new[] { "rabbit@rab5", "rabbit@rab6" }, policy.Definition.HaParams.Nodes);
            Assert.AreEqual("mirror", policy.Pattern);
            Assert.AreEqual(HaSyncMode.Automatic, policy.Definition.HaSyncMode);
            Assert.AreEqual(0, policy.Priority);
        }

        [Test]
        public void Should_write_apply_to_properly()
        {
            var serializedMessage = JsonConvert.SerializeObject(new Policy
            {
                Name = "bob",
                Pattern = "foo"
            }, ManagementClient.Settings);
            
            Assert.IsTrue(serializedMessage.Contains("\"apply-to\":\"all\""));

            serializedMessage = JsonConvert.SerializeObject(new Policy
            {
                Name = "bob",
                Pattern = "foo",
                ApplyTo = ApplyMode.Exchanges
            }, ManagementClient.Settings);

            Assert.IsTrue(serializedMessage.Contains("\"apply-to\":\"exchanges\""));

            serializedMessage = JsonConvert.SerializeObject(new Policy
            {
                Name = "bob",
                Pattern = "foo",
                ApplyTo = ApplyMode.Queues
            }, ManagementClient.Settings);
            Assert.IsTrue(serializedMessage.Contains("\"apply-to\":\"queues\""));
        }

        [Test]
        public void Should_write_federation_upstream_properly()
        {
            var serializedMessage = JsonConvert.SerializeObject(new Policy
            {
                Name = "bob",
                Pattern = "foo",
                Definition = new PolicyDefinition { FederationUpstream = "my-upstream"}
            }, ManagementClient.Settings);

            Assert.IsTrue(serializedMessage.Contains("\"federation-upstream\":\"my-upstream\""));
        }

        [Test]
        public void Should_write_all_ha_policy_without_param()
        {
            var serializedMessage = JsonConvert.SerializeObject(new Policy
            {
                Name = "bob",
                Pattern = "foo",
                Definition = new PolicyDefinition {HaMode = HaMode.All}
            }, ManagementClient.Settings);
            Console.WriteLine(serializedMessage);
            Assert.IsFalse(serializedMessage.Contains("ha-params"));
            Assert.IsFalse(serializedMessage.Contains("federation_upstream_set"));
        }

    }
}