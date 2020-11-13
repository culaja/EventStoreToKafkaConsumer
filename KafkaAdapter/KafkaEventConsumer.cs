using System;
using Confluent.Kafka;
using Framework;
using Ports;

namespace KafkaAdapter
{
    internal sealed class KafkaEventConsumer : IAmEventConsumer
    {
        private ConsumingResultDelegate _consumingResultDelegate = result => { };
        private readonly IProducer<string, string> _producer;

        public KafkaEventConsumer(IProducer<string, string> producer)
        {
            _producer = producer;
        }

        public void RegisterOnConsumingResults(ConsumingResultDelegate consumingResultDelegate)
        {
            _consumingResultDelegate = consumingResultDelegate;
        }

        public void Consume(EventEnvelope eventEnvelope)
        {   
            _producer.Produce(
                eventEnvelope.TopicName,
                new Message<string, string>
                {
                    Key = eventEnvelope.PartitioningKey,
                    Value = eventEnvelope.ToDto().Serialize()
                },
                DeliveryHandler);
        }
        
        private void DeliveryHandler(DeliveryReport<string, string> report)
        {
            var consumingResult = new ConsumingResult(
                EventPosition.Of(EventEnvelopeDto.Deserialize(report.Message.Value).EventPosition),
                report.Error.IsError ? $"{report.Error.Code}: {report.Error.Reason}": Optional<string>.None);
            _consumingResultDelegate(consumingResult);
        }
    }
}