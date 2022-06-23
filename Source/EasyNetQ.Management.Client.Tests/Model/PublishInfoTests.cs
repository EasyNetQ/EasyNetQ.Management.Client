// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using EasyNetQ.Management.Client.Model;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model
{
    public class PublishInfoTests
    {
        [Fact]
        public void Should_throw_when_payload_encoding_is_incorrect()
        {
            //payloadEncoding must be one of: 'string, base64'
            Assert.Throws<ArgumentException>(() => new PublishInfo(new Dictionary<string, object>(), "routing_key", "payload", "unknown_payload_encoding"));
        }
    }
}

// ReSharper restore InconsistentNaming
