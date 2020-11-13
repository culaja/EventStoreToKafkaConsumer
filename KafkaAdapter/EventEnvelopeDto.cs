using Framework;
using Newtonsoft.Json;

namespace KafkaAdapter
{
    internal static class EventEnvelopeExtensions
    {
        public static EventEnvelopeDto ToDto(this EventEnvelope eventEnvelope) =>
            new EventEnvelopeDto(
                eventEnvelope.TopicName,
                eventEnvelope.PartitioningKey,
                eventEnvelope.EventPosition,
                eventEnvelope.EventData,
                eventEnvelope.EventMetaData);
    }
    
    internal sealed class EventEnvelopeDto
    {
        public string TopicName { get; }
        public string PartitioningKey { get; }
        public long[] EventPosition { get; }
        public byte[] EventData { get; }
        public byte[] EventMetaData { get; }
        
        public EventEnvelopeDto(
            string topicName,
            string partitioningKey,
            long[] eventPosition,
            byte[] eventData,
            byte[] eventMetaData)
        {
            TopicName = topicName;
            PartitioningKey = partitioningKey;
            EventPosition = eventPosition;
            EventData = eventData;
            EventMetaData = eventMetaData;
        }
        
        public EventEnvelope ToDomain() => new EventEnvelope(
            Framework.TopicName.Of(TopicName),
            Framework.PartitioningKey.Of(PartitioningKey),
            Framework.EventPosition.Of(EventPosition),
            EventData,
            EventMetaData);

        public static EventEnvelopeDto Deserialize(string data) =>
            JsonConvert.DeserializeObject<EventEnvelopeDto>(data);

        public string Serialize() => JsonConvert.SerializeObject(this);
    }
}