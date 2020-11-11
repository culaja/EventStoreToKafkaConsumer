using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Framework;
using Ports;

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
        
        public async Task Consume(IReadOnlyList<EventEnvelope> eventEnvelopes)
        {
            foreach (var eventEnvelope in eventEnvelopes)
            {
                await Produce(eventEnvelope);
            }
        }

        private async Task Produce(EventEnvelope eventEnvelope)
        {
            try
            {
                await _producer.ProduceAsync(eventEnvelope.TopicName, new Message<string, string>
                {
                    Key = eventEnvelope.PartitioningKey,
                    Value = eventEnvelope.Serialize()
                });
            }
            catch (ProduceException<string, string> ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}