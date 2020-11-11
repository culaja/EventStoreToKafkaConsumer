using System;
using System.Collections.Generic;
using Framework;

namespace Ports
{
    public delegate void EventConsumerDelegate(IReadOnlyList<EventEnvelope> eventEnvelopes);
    
    public interface IAmSubscription : IDisposable
    {
        TopicName TopicName { get; }
        EventPosition EventStartPosition { get; }
    }
    
    public interface IAmEventSubscriptionProvider
    {
        IAmSubscription Subscribe(TopicName topicName, EventPosition eventPosition);
    }
}