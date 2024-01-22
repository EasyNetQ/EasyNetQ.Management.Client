#if NETFRAMEWORK
using System;
using System.Reflection;
#endif

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
    [InlineData("localhost")]
    [InlineData("127.0.0.1")]
    [InlineData("[::1]")]
    [InlineData("[2001:db8:1111::50]")]
    public void Host_url_should_be_legal(string url)
    {
#pragma warning disable CS0618
        using var client = new ManagementClient(url, "user", "password");
#pragma warning restore CS0618
    }

    [Theory]
    [InlineData("::1")]
    [InlineData("2001:db8:1111::50")]
    [InlineData("[[2001:db8:1111::50]]")]
    public void Host_url_should_be_illegal(string url)
    {
#pragma warning disable CS0618
        var exception = Assert.Throws<ArgumentException>(() => new ManagementClient(url, "user", "password"));
#pragma warning restore CS0618

        exception.Message.Should().Be("hostUrl is illegal");
    }

#if NETFRAMEWORK
    [Fact]
    public void UriParser_quirks_should_be_fixed()
    {
        // Enable legacy quirks for https. Must be done before first creation of ManagementClient with https.
        string scheme = Uri.UriSchemeHttps;

        // https://mikehadlow.blogspot.com/2011/08/how-to-stop-systemuri-un-escaping.html
        var getSyntaxMethod =
            typeof(UriParser).GetMethod("GetSyntax", BindingFlags.Static | BindingFlags.NonPublic);
        if (getSyntaxMethod == null)
        {
            throw new MissingMethodException("UriParser", "GetSyntax");
        }
        var uriParser = getSyntaxMethod.Invoke(null, new object[] { scheme })!;

        var setUpdatableFlagsMethod =
            uriParser.GetType().GetMethod("SetUpdatableFlags", BindingFlags.Instance | BindingFlags.NonPublic);
        if (setUpdatableFlagsMethod == null)
        {
            throw new MissingMethodException("UriParser", "SetUpdatableFlags");
        }
        setUpdatableFlagsMethod.Invoke(uriParser, new object[] { 0x2000000 });

        string uriWithEscapedDotsAndSlashes = $"{scheme}://localhost/{Uri.EscapeDataString("/.")}";
        new Uri(uriWithEscapedDotsAndSlashes).ToString().Should().NotBe(uriWithEscapedDotsAndSlashes);

        new ManagementClient(new Uri($"{scheme.ToUpper()}://localhost:15672"), "guest", "guest");

        new Uri(uriWithEscapedDotsAndSlashes).ToString().Should().Be(uriWithEscapedDotsAndSlashes);
    }
#endif
}
