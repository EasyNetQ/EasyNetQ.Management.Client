// ReSharper disable InconsistentNaming

using EasyNetQ.Management.Client.Model;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model
{
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
            topicPermissionInfo.GetUserName().ShouldEqual(user.Name);
        }

        [Fact]
        public void Should_return_the_correct_vhost_name()
        {
            topicPermissionInfo.GetVirtualHostName().ShouldEqual(vhost.Name);
        }

        [Fact]
        public void Should_set_default_exchange_to_null()
        {
            topicPermissionInfo.Exchange.ShouldBeNull();
        }

        [Fact]
        public void Should_set_default_permissions_to_allow_all()
        {
            topicPermissionInfo.Write.ShouldEqual(".*");
            topicPermissionInfo.Read.ShouldEqual(".*");
        }

        [Fact]
        public void Should_be_able_to_set_deny_permissions()
        {
            var topicPermissions = topicPermissionInfo.DenyAllRead().DenyAllWrite();

            topicPermissions.Write.ShouldEqual("^$");
            topicPermissions.Read.ShouldEqual("^$");
        }

        [Fact]
        public void Should_be_able_to_set_arbitrary_permissions()
        {
            var topicPermissions = topicPermissionInfo.SetRead("def").SetWrite("xyz");

            topicPermissions.Write.ShouldEqual("xyz");
            topicPermissions.Read.ShouldEqual("def");
        }

        [Fact]
        public void Should_be_able_to_set_arbitrary_exchange()
        {
            var topicPermissions = topicPermissionInfo.SetExchangeType("amq.topic");

            topicPermissions.Exchange.ShouldEqual("amq.topic");
        }
    }
}

// ReSharper restore InconsistentNaming
