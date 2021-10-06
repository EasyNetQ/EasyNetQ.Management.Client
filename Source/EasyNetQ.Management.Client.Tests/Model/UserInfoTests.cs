// ReSharper disable InconsistentNaming

using System;
using EasyNetQ.Management.Client.Model;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model
{
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
        public void Should_have_correct_name_and_password()
        {
            userInfo.GetName().Should().Be(userName);
            userInfo.Password.Should().Be(password);
        }

        [Fact]
        public void Should_be_able_to_add_tags()
        {
            userInfo.AddTag("administrator").AddTag("management");
            userInfo.Tags.Should().Contain("administrator", "management");
        }

        [Fact]
        public void Should_not_be_able_to_add_the_same_tag_twice()
        {
            Assert.Throws<ArgumentException>(() => userInfo.AddTag("management").AddTag("management"));
        }

        [Fact]
        public void Should_not_be_able_to_add_incorrect_tags()
        {
            Assert.Throws<ArgumentException>(() => userInfo.AddTag("blah"));
        }

        [Fact]
        public void Should_have_a_default_tag_of_empty_string()
        {
            userInfo.Tags.Should().BeEmpty();
        }
    }
}

// ReSharper restore InconsistentNaming
