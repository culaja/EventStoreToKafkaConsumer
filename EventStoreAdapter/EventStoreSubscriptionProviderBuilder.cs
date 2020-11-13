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
            string positionStoreStreamName,
            string filterPattern)
        {
            _eventStoreConnection = eventStoreConnection;
            EventSubscriptionProvider = new EventStoreSubscriptionProvider(
                _eventStoreConnection,
                userCredentials,
                positionStoreStreamName,
                filterPattern);
        }

        public static EventStoreSubscriptionProviderBuilder NewUsing(
            string eventStoreConnectionString,
            string userName,
            string password,
            string positionStoreStreamName,
            string filerPattern)
        {
            var connection = EventStoreConnection.Create(
                ConnectionSettings.Create()
                    .KeepReconnecting()
                    .KeepRetrying(),
                new Uri(eventStoreConnectionString));
            
            connection.ConnectAsync().Wait();
            
            return new EventStoreSubscriptionProviderBuilder(
                connection,
                new UserCredentials(userName, password),
                positionStoreStreamName,
                filerPattern);
        }

        public IAmEventSubscriptionProvider EventSubscriptionProvider { get; }
        
        public void Dispose()
        {
            _eventStoreConnection?.Dispose();
        }
    }
}