using System;
using EventStoreAdapter;
using Framework;

namespace EventStoreSubscriptionTestApp
{
    public static class Program
    {   
        public static void Main()
        {
            using var builder = EventStoreSubscriptionProviderBuilder.NewUsing(
                "tcp://admin:changeit@localhost:1113",
                "positionStoreStreamName",
                "Domain");

            using var subscription = builder.EventSubscriptionProvider.Subscribe(
                TopicName.AllEvents,
                NewEventEnvelopeReceivedHandler,
                LiveProcessingStartedHandler,
                SubscriptionDroppedHandler);

            Console.ReadLine();
        }

        private static void NewEventEnvelopeReceivedHandler(EventEnvelope eventEnvelope)
        {
            Console.WriteLine(eventEnvelope);
        }

        private static void SubscriptionDroppedHandler(string reason, Exception exception)
        {
            Console.WriteLine($"Subscription dropped: {reason}");
        }

        private static void LiveProcessingStartedHandler()
        {
            Console.WriteLine();
            Console.WriteLine($"*** Live processing started ***");
            Console.WriteLine();
        }
    }
}