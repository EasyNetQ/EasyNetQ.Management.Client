
using FluentAssertions;

using Xunit;

namespace EasyNetQ.Management.Client.Tests;

public class ManagementClientConstructorTests
{
    [Fact]
    public void Host_should_not_be_null()
    {
        var exception = Assert.Throws<ArgumentException>(() => new ManagementClient(string.Empty, "user", "password"));

        exception.Message.Should().Be("hostUrl is null or empty");
    }

    [Fact]
    public void Host_should_have_correct_url_for_ssl()
    {
        var exception = Assert.Throws<ArgumentException>(() => new ManagementClient("http://localhost", "user", "password", ssl: true));

        exception.Message.Should().Be("hostUrl is illegal");
    }

    [Theory]
    [InlineData("localhost", true)]
    [InlineData("127.0.0.1", true)]
    [InlineData("::1", false)]
    [InlineData("2001:db8:1111::50", false)]
    [InlineData("[::1]", true)]
    [InlineData("[2001:db8:1111::50]", true)]
    [InlineData("[[2001:db8:1111::50]]", false)]
    public void Host_url_should_be_legal(string url, bool isValid)
    {
        var exception = Record.Exception(() => new ManagementClient(url, "user", "password"));

        if (isValid)
        {
            exception.Should().BeNull();
        }
        else
        {
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("hostUrl is illegal");
        }
    }
}
