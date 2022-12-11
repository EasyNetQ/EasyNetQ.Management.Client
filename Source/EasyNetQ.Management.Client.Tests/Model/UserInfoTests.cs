// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model;

public class UserInfoTests
{
    private readonly UserInfo userInfo;
    private const string userName = "mike";
    private const string password = "topSecret";

    public UserInfoTests()
    {
        userInfo = new UserInfo(userName, password);
    }

    [Fact]
    public void Should_be_able_to_add_tags()
    {
        userInfo.AddTag(UserTag.Administrator).AddTag(UserTag.Management);
        userInfo.Tags.Should().BeEquivalentTo(new[] { UserTag.Administrator, UserTag.Management });
    }

    [Fact]
    public void Should_have_a_default_tag_of_empty_string()
    {
        userInfo.Tags.Should().BeEmpty();
    }
}

// ReSharper restore InconsistentNaming
