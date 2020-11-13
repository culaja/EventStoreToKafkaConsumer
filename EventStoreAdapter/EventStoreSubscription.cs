using EventStore.ClientAPI;
using Framework;
using Ports;

namespace EventStoreAdapter
{
    internal sealed class EventStoreSubscription : IAmEventSubscription
    {
        private readonly EventStoreCatchUpSubscription _catchUpSubscription;
        private readonly string _subscriptionName;

        public EventStoreSubscription(
            TopicName topicName,
            EventPosition eventStartPosition,
            EventStoreCatchUpSubscription catchUpSubscription,
            string subscriptionName)
        {
            TopicName = topicName;
            EventStartPosition = eventStartPosition;
            _catchUpSubscription = catchUpSubscription;
            _subscriptionName = subscriptionName;
        }

        public TopicName TopicName { get; }
        public EventPosition EventStartPosition { get; }
        
        public void AckConsumed(EventPosition eventPosition)
        {
        }
        
        public void Dispose()
        {
            _catchUpSubscription?.Stop();
        }

        public override string ToString()
        {
            return $"{nameof(_subscriptionName)}: {_subscriptionName}, {nameof(TopicName)}: {TopicName}, {nameof(EventStartPosition)}: {EventStartPosition}";
        }
    }
}