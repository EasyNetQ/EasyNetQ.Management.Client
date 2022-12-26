namespace EasyNetQ.Management.Client.Tests;

public class ManagementClientConstructorTests
{
    [Fact]
    public void Host_should_not_be_null()
    {
#pragma warning disable CS0618
        var exception = Assert.Throws<ArgumentException>(() => new ManagementClient(string.Empty, "user", "password"));
#pragma warning restore CS0618

        exception.Message.Should().Be("hostUrl is null or empty");
    }

    [Fact]
    public void Host_should_have_correct_url_for_ssl()
    {
#pragma warning disable CS0618
        var exception = Assert.Throws<ArgumentException>(() => new ManagementClient("http://localhost", "user", "password", ssl: true));
#pragma warning restore CS0618

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
#pragma warning disable CS0618
        var exception = Record.Exception(() => new ManagementClient(url, "user", "password"));
#pragma warning restore CS0618

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
