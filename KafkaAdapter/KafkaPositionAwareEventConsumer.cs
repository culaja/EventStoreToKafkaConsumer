using System.Collections.Generic;
using Confluent.Kafka;
using Framework;
using Ports;
using static Framework.Optional<Confluent.Kafka.DeliveryReport<string,string>>;

namespace KafkaAdapter
{
    internal sealed class KafkaPositionAwareEventConsumer : IAmPositionAwareEventConsumer
    {
        private readonly IProducer<string, string> _producer;

        public KafkaPositionAwareEventConsumer(IProducer<string, string> producer)
        {
            _producer = producer;
        }

        public EventPosition LastKnownEventPositionFor(TopicName topicName)
        {
            return EventPosition.Of(0);
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
                    Value = eventEnvelope.Serialize()
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