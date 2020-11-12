using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Framework;
using Ports;
using static Confluent.Kafka.ErrorCode;
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
            try
            {
                using var consumer = _consumerBuilder.Build();
                consumer.Subscribe(topicName);
                var consumeResult = consumer.Consume(TimeSpan.FromSeconds(5));
                var eventEnvelope = EventEnvelopeDto.Deserialize(consumeResult.Message.Value).ToDomain();
                return eventEnvelope.EventPosition;
            }
            catch (ConsumeException ex)
            {
                switch (ex)
                {
                    case var e when e.Error.Code == UnknownTopicOrPart:
                        return EventPosition.Beginning;
                    default:
                        throw;
                }
            }
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