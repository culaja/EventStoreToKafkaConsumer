using System.Collections.Generic;

namespace Framework
{
    public sealed class EventEnvelope : ValueObject
    {
        public TopicName TopicName { get; }
        public PartitioningKey PartitioningKey { get; }
        public EventPosition EventPosition { get; }
        public string SerializedEvent { get; }

        public EventEnvelope(
            TopicName topicName,
            PartitioningKey partitioningKey,
            EventPosition eventPosition,
            string serializedEvent)
        {
            TopicName = topicName;
            PartitioningKey = partitioningKey;
            EventPosition = eventPosition;
            SerializedEvent = serializedEvent;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TopicName;
            yield return PartitioningKey;
            yield return EventPosition;
            yield return SerializedEvent;
        }
    }
}