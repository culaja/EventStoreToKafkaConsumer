using EventStore.ClientAPI;
using Framework;

namespace EventStoreAdapter
{
    internal static class ConversionExtensions
    {
        public static Optional<EventEnvelope> ToOptionalEventEnvelopeWith(
            this ResolvedEvent resolvedEvent,
            TopicName topicName,
            string filterPattern)
        {
            return resolvedEvent.OriginalStreamId.ToOptionalPartitioningKey(filterPattern).Map(partitioningKey =>
                new EventEnvelope(
                    topicName,
                    partitioningKey,
                    resolvedEvent.OriginalPosition.ToEventPosition(),
                    resolvedEvent.Event.Data,
                    resolvedEvent.Event.Metadata));
        }

        private static Optional<PartitioningKey> ToOptionalPartitioningKey(
            this string originalStreamId,
            string filterPattern)
        {
            var array = originalStreamId.Split('|');
            if (array.Length > 1 && array[0] == filterPattern)
            {
                return PartitioningKey.Of(array[1]); 
            }

            return Optional<PartitioningKey>.None;
        }

        private static EventPosition ToEventPosition(this Position? position) =>
            position.HasValue
                ? EventPosition.Of(position.Value.CommitPosition, position.Value.PreparePosition)
                : EventPosition.Beginning;
    }
}