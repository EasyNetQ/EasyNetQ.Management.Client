// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model;

public class TopicPermissionInfoTests
{
    private TopicPermissionInfo topicPermissionInfo;
    private User user;
    private Vhost vhost;

    public TopicPermissionInfoTests()
    {
        user = new User { Name = "mikey" };
        vhost = new Vhost { Name = "theVHostName" };
        topicPermissionInfo = new TopicPermissionInfo(user, vhost);
    }

    [Fact]
    public void Should_return_the_correct_user_name()
    {
        topicPermissionInfo.GetUserName().Should().Be(user.Name);
    }

    [Fact]
    public void Should_return_the_correct_vhost_name()
    {
        topicPermissionInfo.GetVirtualHostName().Should().Be(vhost.Name);
    }

    [Fact]
    public void Should_set_default_exchange_to_null()
    {
        topicPermissionInfo.Exchange.Should().BeNull();
    }

    [Fact]
    public void Should_set_default_permissions_to_allow_all()
    {
        topicPermissionInfo.Write.Should().Be(".*");
        topicPermissionInfo.Read.Should().Be(".*");
    }

    [Fact]
    public void Should_be_able_to_set_deny_permissions()
    {
        var topicPermissions = topicPermissionInfo.DenyAllRead().DenyAllWrite();

        topicPermissions.Write.Should().Be("^$");
        topicPermissions.Read.Should().Be("^$");
    }

    [Fact]
    public void Should_be_able_to_set_arbitrary_permissions()
    {
        var topicPermissions = topicPermissionInfo.SetRead("def").SetWrite("xyz");

        topicPermissions.Write.Should().Be("xyz");
        topicPermissions.Read.Should().Be("def");
    }

    [Fact]
    public void Should_be_able_to_set_arbitrary_exchange()
    {
        var topicPermissions = topicPermissionInfo.SetExchangeType("amq.topic");

        topicPermissions.Exchange.Should().Be("amq.topic");
    }
}

// ReSharper restore InconsistentNaming
