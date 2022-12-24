// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model;

public class TopicPermissionInfoTests
{
    private TopicPermissionInfo topicPermissionInfo;

    public TopicPermissionInfoTests()
    {
        topicPermissionInfo = new TopicPermissionInfo("mikey");
    }

    [Fact]
    public void Should_return_the_correct_user_name()
    {
        topicPermissionInfo.UserName.Should().Be("mikey");
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
