using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Framework;
using Ports;

namespace EventStoreAdapter
{
    internal sealed class EventStoreSubscriptionProvider : IAmEventSubscriptionProvider
    {
        private readonly IEventStoreConnection _connection;
        private readonly UserCredentials _userCredentials;
        private readonly string _subscriptionName;

        public EventStoreSubscriptionProvider(
            IEventStoreConnection connection,
            UserCredentials userCredentials,
            string subscriptionName)
        {
            _connection = connection;
            _userCredentials = userCredentials;
            _subscriptionName = subscriptionName;
        }
        
        public IAmEventSubscription Subscribe(
            TopicName topicName,
            NewEventEnvelopeReceivedDelegate newEventEnvelopeReceivedHandler,
            LiveProcessingStartedDelegate liveProcessingStartedHandler,
            SubscriptionDroppedDelegate subscriptionDroppedHandler)
        {
            var topicPosition = EventPosition.Beginning;
            
            var catchUpSubscription = _connection.SubscribeToAllFrom(
                Position.Start, 
                CatchUpSubscriptionSettings.Default, 
                (_, e) => newEventEnvelopeReceivedHandler(e.ToEventEnvelopeWith(topicName)),
                _ => liveProcessingStartedHandler(),
                (_, r, ex) => subscriptionDroppedHandler(r.ToString(), ex),
                _userCredentials);
            
            return new EventStoreSubscription(
                topicName, 
                topicPosition,
                catchUpSubscription,
                _subscriptionName);
        }
    }
}