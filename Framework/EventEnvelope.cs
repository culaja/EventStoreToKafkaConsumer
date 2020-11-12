using System.Collections.Generic;
using Newtonsoft.Json;

namespace Framework
{
    public sealed class EventEnvelope : ValueObject
    {
        public TopicName TopicName { get; }
        public PartitioningKey PartitioningKey { get; }
        public EventPosition EventPosition { get; }
        public IAmEvent Event { get; }

        public EventEnvelope(
            TopicName topicName,
            PartitioningKey partitioningKey,
            EventPosition eventPosition,
            IAmEvent @event)
        {
            TopicName = topicName;
            PartitioningKey = partitioningKey;
            EventPosition = eventPosition;
            Event = @event;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TopicName;
            yield return PartitioningKey;
            yield return EventPosition;
            yield return Event;
        }

        public string Serialize() => JsonConvert.SerializeObject(this);

        public static EventEnvelope Deserialize(string data) => 
            JsonConvert.DeserializeObject<EventEnvelope>(data);
    }
}