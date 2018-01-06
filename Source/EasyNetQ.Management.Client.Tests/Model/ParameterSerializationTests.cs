using EasyNetQ.Management.Client.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model
{
    public class ParameterSerializationTests
    {
        [Fact]
        public void Should_be_able_to_serialize_arbitrary_object_as_value()
        {
            var serializedvalue =
                JsonConvert.SerializeObject(new Parameter
                {
                    Vhost = "/",
                    Component = "bob",
                    Name = "test",
                    Value = new Policy {Pattern = "testvalue"}
                }, ManagementClient.Settings);
            var parsedValue = JObject.Parse(serializedvalue);
            Assert.Equal("testvalue", parsedValue["value"]["pattern"].Value<string>());
        }
    }
}