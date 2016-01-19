// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using EasyNetQ.Management.Client.Model;
using NUnit.Framework;

namespace EasyNetQ.Management.Client.Tests.Model
{
    [TestFixture(Category = "Unit")]
    public class QueueSerializationTests
    {
        private List<Queue> queues;

        [Test]
        public void BackingQueueStatus_With_NextSeqId_Exceeding_IntMaxLength_Can_Be_Deserialized()
        {
            queues = ResourceLoader.LoadObjectFromJson<List<Queue>>("Queues.json");

            var queue = queues[1];

            queue.BackingQueueStatus.NextSeqId.ShouldEqual(((long)int.MaxValue) + 1);
        }

        [Test]
        public void NonInt_Consumer_Details_Channel_Details_Peer_Port_Can_Be_Deserialize()
        {
            queues = ResourceLoader.LoadObjectFromJson<List<Queue>>("Queues.json");

            var queue = queues[2];

            queue.ConsumerDetails[0].ChannelDetails.PeerPort.ShouldEqual(0);
        }

        [Test]
        public void Int_Consumer_Details_Channel_Details_Peer_Port_Can_Be_Deserialize()
        {
            queues = ResourceLoader.LoadObjectFromJson<List<Queue>>("Queues.json");

            var queue = queues[3];

            queue.ConsumerDetails[0].ChannelDetails.PeerPort.ShouldEqual(9861);
        }
    }
}

// ReSharper restore InconsistentNaming