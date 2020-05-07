// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model
{
    public class PermissionInfoTests
    {
        private PermissionInfo permissionInfo;
        private User user;
        private Vhost vhost;

        public PermissionInfoTests()
        {
            user = new User { Name = "mikey" };
            vhost = new Vhost { Name = "theVHostName" };
            permissionInfo = new PermissionInfo(user, vhost);
        }

        [Fact]
        public void Should_return_the_correct_user_name()
        {
            permissionInfo.GetUserName().Should().Be(user.Name);
        }

        [Fact]
        public void Should_return_the_correct_vhost_name()
        {
            permissionInfo.GetVirtualHostName().Should().Be(vhost.Name);
        }

        [Fact]
        public void Should_set_default_permissions_to_allow_all()
        {
            permissionInfo.Configure.Should().Be(".*");
            permissionInfo.Write.Should().Be(".*");
            permissionInfo.Read.Should().Be(".*");
        }

        [Fact]
        public void Should_be_able_to_set_deny_permissions()
        {
            var permissions = permissionInfo.DenyAllConfigure().DenyAllRead().DenyAllWrite();

            permissions.Configure.Should().Be("^$");
            permissions.Write.Should().Be("^$");
            permissions.Read.Should().Be("^$");
        }

        [Fact]
        public void Should_be_able_to_set_arbitrary_permissions()
        {
            var permissions = permissionInfo.SetConfigure("abc").SetRead("def").SetWrite("xyz");

            permissions.Configure.Should().Be("abc");
            permissions.Write.Should().Be("xyz");
            permissions.Read.Should().Be("def");
        }
    }
}

// ReSharper restore InconsistentNaming
