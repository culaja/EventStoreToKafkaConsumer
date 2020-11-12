using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Framework;
using Ports;
using static Framework.Optional<Confluent.Kafka.DeliveryReport<string,string>>;

namespace KafkaAdapter
{
    internal sealed class KafkaPositionAwareEventConsumer : IAmPositionAwareEventConsumer
    {
        private readonly IProducer<string, string> _producer;
        private readonly ConsumerBuilder<string, string> _consumerBuilder;

        public KafkaPositionAwareEventConsumer(
            IProducer<string, string> producer,
            ConsumerBuilder<string, string> consumerBuilder)
        {
            _producer = producer;
            _consumerBuilder = consumerBuilder;
        }

        public EventPosition LastKnownEventPositionFor(TopicName topicName)
        {
            using var consumer = _consumerBuilder.Build();
            consumer.Subscribe(topicName);
            var lastEventEnvelopes = consumer.ConsumeLastEventEnvelopes();
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