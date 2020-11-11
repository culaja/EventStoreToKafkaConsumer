using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;

namespace Ports
{
    public sealed class ConsumerOutOfSyncException : Exception
    {
        public ConsumerOutOfSyncException() : base("Consumer is out of sync. Restart sending events from a last known position.")
        {
        }
    }

    public interface IAmPositionAwareEventConsumer
    {
        EventPosition LastKnownEventPositionFor(TopicName topicName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventEnvelopes"></param>
        /// <exception cref="ConsumerOutOfSyncException">
        /// Thrown if received <see cref="eventEnvelopes"/> positions are behind <see cref="LastKnownEventPositionFor"/>.
        /// </exception>
        Task Consume(IReadOnlyList<EventEnvelope> eventEnvelopes);
    }
}