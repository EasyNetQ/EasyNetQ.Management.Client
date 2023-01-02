using System.Text.Json;
using EasyNetQ.Management.Client.Model;
using Xunit.Abstractions;

namespace EasyNetQ.Management.Client.Tests.Model;

public class ParameterSerializationTests
{
    private readonly ITestOutputHelper output;

    public ParameterSerializationTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Should_be_able_to_serialize_arbitrary_object_as_value()
    {
        var serializedValue = JsonSerializer.Serialize(
            new Parameter(
                Vhost: "/",
                Component: "bob",
                Name: "test",
                Value: new Policy { Pattern = "testvalue" }
            ),
            ManagementClient.SerializerOptions
        );
        output.WriteLine(serializedValue);
        // Assert.Equal("testvalue", parsedValue["value"]["pattern"].Value<string>());
    }
}
