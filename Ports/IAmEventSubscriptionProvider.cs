using System;
using System.Collections.Generic;
using Framework;

namespace Ports
{
    public delegate void NewEventEnvelopeReceivedDelegate(EventEnvelope eventEnvelope);
    public delegate void LiveProcessingStartedDelegate();
    public delegate void SubscriptionDroppedDelegate(string reason, Exception exception);
    
    public interface IAmEventSubscription : IDisposable
    {
        TopicName TopicName { get; }
        EventPosition EventStartPosition { get; }
        void AckConsumed(EventPosition eventPosition);
    }
    
    public interface IAmEventSubscriptionProvider
    {
        IAmEventSubscription Subscribe(
            TopicName topicName,
            NewEventEnvelopeReceivedDelegate newEventEnvelopeReceivedHandler,
            LiveProcessingStartedDelegate liveProcessingStartedHandler,
            SubscriptionDroppedDelegate subscriptionDroppedHandler);
    }
}