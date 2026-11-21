#if NETFRAMEWORK
using System;
using System.Net.Http;
using System.Reflection;
#endif

namespace EasyNetQ.Management.Client.Tests;

public class ManagementClientConstructorTests
{
    [Fact]
    public void Endpoint_should_be_absolute()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new ManagementClient(new Uri("/api", UriKind.Relative), "user", "password"));

        exception.ParamName.Should().Be("endpoint");
    }

    [Fact]
    public void HttpClient_should_have_a_base_address()
    {
        using var httpClient = new HttpClient();

        Assert.Throws<ArgumentNullException>(() => new ManagementClient(httpClient, "user", "password"));
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
