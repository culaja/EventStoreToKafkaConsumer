using EventStore.ClientAPI;
using Framework;

namespace EventStoreAdapter
{
    internal static class ConversionExtensions
    {
        public static EventEnvelope ToEventEnvelopeWith(this ResolvedEvent resolvedEvent, TopicName topicName)
        {
            return new EventEnvelope(
                topicName,
                resolvedEvent.OriginalStreamId.ToPartitioningKey(),
                resolvedEvent.OriginalPosition.ToEventPosition(),
                resolvedEvent.Event.Data,
                resolvedEvent.Event.Metadata);
        }

        private static PartitioningKey ToPartitioningKey(this string originalStreamId) => 
            PartitioningKey.Of(originalStreamId.Split('|')[0]);

        private static EventPosition ToEventPosition(this Position? position) =>
            position.HasValue
                ? EventPosition.Of(position.Value.CommitPosition, position.Value.PreparePosition)
                : EventPosition.Beginning;
    }
}