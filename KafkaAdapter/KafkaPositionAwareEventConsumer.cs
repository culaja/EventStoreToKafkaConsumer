using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Framework;
using KafkaAdapter.ConsumingPosition;
using Ports;
using static Framework.Optional<Confluent.Kafka.DeliveryReport<string,string>>;

namespace KafkaAdapter
{
    internal sealed class KafkaPositionAwareEventConsumer : IAmPositionAwareEventConsumer
    {
        private readonly IProducer<string, string> _producer;
        private readonly LastMessageConsumer _lastMessageConsumer;

        public KafkaPositionAwareEventConsumer(
            IProducer<string, string> producer,
            LastMessageConsumer lastMessageConsumer)
        {
            _producer = producer;
            _lastMessageConsumer = lastMessageConsumer;
        }

        public EventPosition LastKnownEventPositionFor(TopicName topicName)
        {
            var lastEventEnvelopes = _lastMessageConsumer.ConsumeLastEventEnvelopesFor(topicName);
            return lastEventEnvelopes
                .Select(ee => ee.EventPosition)
                .DefaultIfEmpty(EventPosition.Beginning)
                .Max();
        }
        
        public void Consume(IReadOnlyList<EventEnvelope> eventEnvelopes)
        {
            Optional<DeliveryReport<string, string>> optionalFirstFailedDeliveryReport = None;
            void DeliveryHandler(DeliveryReport<string, string> report)
            {
                if (report.Error.IsError && optionalFirstFailedDeliveryReport.HasNoValue)
                {
                    optionalFirstFailedDeliveryReport = report;
                }
            }
            
            foreach (var eventEnvelope in eventEnvelopes)
            {
                _producer.Produce(eventEnvelope.TopicName, new Message<string, string>
                {
                    Key = eventEnvelope.PartitioningKey,
                    Value = eventEnvelope.ToDto().Serialize()
                }, DeliveryHandler);
            }

            _producer.Flush();

            if (optionalFirstFailedDeliveryReport.HasValue)
            {
                throw new FailedToProduceKafkaMessageException(optionalFirstFailedDeliveryReport.Value);
            }
        }
    }
}