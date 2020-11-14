using System.Collections.Generic;

namespace Framework
{
    public sealed class EventEnvelope : ValueObject
    {
        public TopicName TopicName { get; }
        public PartitioningKey PartitioningKey { get; }
        public EventPosition EventPosition { get; }
        public byte[] EventData { get; }
        public byte[] EventMetaData { get; }

        public EventEnvelope(
            TopicName topicName,
            PartitioningKey partitioningKey,
            EventPosition eventPosition,
            byte[] eventData,
            byte[] eventMetaData)
        {
            TopicName = topicName;
            PartitioningKey = partitioningKey;
            EventPosition = eventPosition;
            EventData = eventData;
            EventMetaData = eventMetaData;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TopicName;
            yield return PartitioningKey;
            yield return EventPosition;
        }

        public override string ToString()
        {
            return $"{nameof(TopicName)}: {TopicName}, {nameof(PartitioningKey)}: {PartitioningKey}, {nameof(EventPosition)}: {EventPosition}";
        }
    }
}