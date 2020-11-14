using EventStore.ClientAPI;
using Framework;
using Ports;

namespace EventStoreAdapter
{
    internal sealed class EventStoreSubscriptionProvider : IAmEventSubscriptionProvider
    {
        private readonly IEventStoreConnection _connection;
        private readonly string _subscriptionName;
        private readonly string _filterPattern;

        public EventStoreSubscriptionProvider(
            IEventStoreConnection connection,
            string subscriptionName,
            string filterPattern)
        {
            _connection = connection;
            _subscriptionName = subscriptionName;
            _filterPattern = filterPattern;
        }
        
        public IAmEventSubscription Subscribe(
            TopicName topicName,
            NewEventEnvelopeReceivedDelegate newEventEnvelopeReceivedHandler,
            LiveProcessingStartedDelegate liveProcessingStartedHandler,
            SubscriptionDroppedDelegate subscriptionDroppedHandler)
        {
            var topicPosition = EventPosition.Beginning;

            void EventAppeared(EventStoreCatchUpSubscription _, ResolvedEvent e)
            {
                var optional = e.ToOptionalEventEnvelopeWith(topicName, _filterPattern);
                if (optional.HasValue)
                {
                    newEventEnvelopeReceivedHandler(optional.Value);
                }
            }

            var catchUpSubscription = _connection.SubscribeToAllFrom(
                null, 
                CatchUpSubscriptionSettings.Default, 
                EventAppeared,
                _ => liveProcessingStartedHandler(),
                (_, r, ex) => subscriptionDroppedHandler(r.ToString(), ex));
            
            return new EventStoreSubscription(
                topicName, 
                topicPosition,
                catchUpSubscription,
                _subscriptionName);
        }
    }
}