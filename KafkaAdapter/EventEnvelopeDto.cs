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
                eventEnvelope.SerializedEvent);
    }
    
    internal sealed class EventEnvelopeDto
    {
        public string TopicName { get; }
        public string PartitioningKey { get; }
        public ulong EventPosition { get; }
        public string SerializedEvent { get; }
        
        public EventEnvelopeDto(
            string topicName,
            string partitioningKey,
            ulong eventPosition,
            string serializedEvent)
        {
            TopicName = topicName;
            PartitioningKey = partitioningKey;
            EventPosition = eventPosition;
            SerializedEvent = serializedEvent;
        }
        
        public EventEnvelope ToDomain() => new EventEnvelope(
            Framework.TopicName.Of(TopicName),
            Framework.PartitioningKey.Of(PartitioningKey),
            Framework.EventPosition.Of(EventPosition),
            SerializedEvent);

        public static EventEnvelopeDto Deserialize(string data) =>
            JsonConvert.DeserializeObject<EventEnvelopeDto>(data);

        public string Serialize() => JsonConvert.SerializeObject(this);
    }
}