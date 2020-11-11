using System.Collections.Generic;

namespace Framework
{
    public sealed class EventEnvelope : ValueObject
    {
        public TopicName TopicName { get; }
        public PartitionName PartitionName { get; }
        public EventPosition EventPosition { get; }
        public IAmEvent Event { get; }

        public EventEnvelope(
            TopicName topicName,
            PartitionName partitionName,
            EventPosition eventPosition,
            IAmEvent @event)
        {
            TopicName = topicName;
            PartitionName = partitionName;
            EventPosition = eventPosition;
            Event = @event;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TopicName;
            yield return PartitionName;
            yield return EventPosition;
            yield return Event;
        }

        public string Serialize()
        {
            return $"{TopicName}.{PartitionName}";
        }
    }
}