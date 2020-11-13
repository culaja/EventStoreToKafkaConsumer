using System;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Ports;

namespace EventStoreAdapter
{
    public sealed class EventStoreSubscriptionProviderBuilder : IDisposable
    {
        private readonly IEventStoreConnection _eventStoreConnection;

        public EventStoreSubscriptionProviderBuilder(
            IEventStoreConnection eventStoreConnection,
            UserCredentials userCredentials,
            string positionStoreStreamName)
        {
            _eventStoreConnection = eventStoreConnection;
            EventSubscriptionProvider = new EventStoreSubscriptionProvider(
                _eventStoreConnection,
                userCredentials,
                positionStoreStreamName);
        }

        public static EventStoreSubscriptionProviderBuilder NewUsing(
            string eventStoreConnectionString,
            string userName,
            string password,
            string positionStoreStreamName)
        {
            var connection = EventStoreConnection.Create(
                ConnectionSettings.Create().KeepReconnecting(),
                new Uri(eventStoreConnectionString));
            
            connection.ConnectAsync();
            
            return new EventStoreSubscriptionProviderBuilder(
                connection,
                new UserCredentials(userName, password),
                positionStoreStreamName);
        }

        public IAmEventSubscriptionProvider EventSubscriptionProvider { get; }
        
        public void Dispose()
        {
            _eventStoreConnection?.Dispose();
        }
    }
}