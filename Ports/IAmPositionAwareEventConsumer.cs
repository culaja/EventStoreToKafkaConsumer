using System;
using System.Collections.Generic;
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
        /// Consumes passed event envelopes.
        /// </summary>
        /// <exception cref="ConsumerOutOfSyncException">
        /// Thrown if received <see cref="eventEnvelopes"/> positions are behind <see cref="LastKnownEventPositionFor"/>.
        /// </exception>
        void Consume(IReadOnlyList<EventEnvelope> eventEnvelopes);
    }
}