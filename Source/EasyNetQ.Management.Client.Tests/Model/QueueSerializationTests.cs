// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using EasyNetQ.Management.Client.Model;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.Management.Client.Tests.Model;

public class QueueSerializationTests
{
    private List<Queue> queues;

    [Fact]
    public void BackingQueueStatus_With_NextSeqId_Exceeding_IntMaxLength_Can_Be_Deserialized()
    {
        queues = ResourceLoader.LoadObjectFromJson<List<Queue>>("Queues.json");

        var queue = queues[1];

        queue.BackingQueueStatus.NextSeqId.Should().Be(((long)int.MaxValue) + 1);
    }

    [Fact]
    public void NonInt_Consumer_Details_Channel_Details_Peer_Port_Can_Be_Deserialize()
    {
        queues = ResourceLoader.LoadObjectFromJson<List<Queue>>("Queues.json");

        var queue = queues[2];

        queue.ConsumerDetails[0].ChannelDetails.PeerPort.Should().Be(0);
    }

    [Fact]
    public void Int_Consumer_Details_Channel_Details_Peer_Port_Can_Be_Deserialize()
    {
        queues = ResourceLoader.LoadObjectFromJson<List<Queue>>("Queues.json");

        var queue = queues[3];

        queue.ConsumerDetails[0].ChannelDetails.PeerPort.Should().Be(9861);
    }

    [Fact]
    public void NonInt_Consumer_Details_Channel_Details_Number_Can_Be_Deserialized()
    {
        queues = ResourceLoader.LoadObjectFromJson<List<Queue>>("Queues.json");

        var queue = queues[2];

        queue.ConsumerDetails[0].ChannelDetails.Number.Should().Be(0);
    }

    [Fact]
    public void Int_Consumer_Details_Channel_Details_Number_Can_Be_Deserialized()
    {
        queues = ResourceLoader.LoadObjectFromJson<List<Queue>>("Queues.json");

        var queue = queues[3];

        queue.ConsumerDetails[0].ChannelDetails.Number.Should().Be(1);
    }
}

// ReSharper restore InconsistentNaming
