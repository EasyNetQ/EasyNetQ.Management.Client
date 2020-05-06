using System;
using Xunit;

namespace EasyNetQ.Management.Client.IntegrationTests
{
    public static class TestExtensions
    {
        public static T ShouldNotBeNull<T>(this T obj) where T : class
        {
            Assert.NotNull(obj);
            return obj;
        }

        public static T ShouldEqual<T>(this T actual, object expected)
        {
            Assert.Equal(expected, actual);
            return actual;
        }

        public static T ShouldNotEqual<T>(this T actual, object expected)
        {
            Assert.NotEqual(expected, actual);
            return actual;
        }

        public static void ShouldBeTrue(this bool source)
        {
            Assert.True(source);
        }

        public static void ShouldBeFalse(this bool source)
        {
            Assert.False(source);
        }

        public static T ShouldBeGreaterThan<T>(this T actual, T expected, string message = null)
            where T : IComparable
        {
            Assert.True(actual.CompareTo(expected) > 0, message);
            return actual;
        }
    }
}
