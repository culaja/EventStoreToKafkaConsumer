using System;
using Confluent.Kafka;
using KafkaAdapter.ConsumingPosition;
using Ports;
using Partitioner = Confluent.Kafka.Partitioner;

namespace KafkaAdapter
{
    public sealed class KafkaPositionAwareEventConsumerBuilder : IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly LastMessageConsumer _lastMessageConsumer;

        private KafkaPositionAwareEventConsumerBuilder(
            IProducer<string, string> producer,
            LastMessageConsumer lastMessageConsumer)
        {
            _producer = producer;
            _lastMessageConsumer = lastMessageConsumer;
        }

        public IAmPositionAwareEventConsumer PositionAwareEventConsumer => 
            new KafkaPositionAwareEventConsumer(_producer, _lastMessageConsumer);
        
        public static KafkaPositionAwareEventConsumerBuilder NewUsing(string kafkaConnectionString)
        {
            var producerConfig = new ProducerConfig { BootstrapServers = kafkaConnectionString, Partitioner = Partitioner.Consistent};
            var producer = new ProducerBuilder<string, string>(producerConfig).Build();
            
            var lastMessageConsumer = LastMessageConsumer.For(kafkaConnectionString);
            
            return new KafkaPositionAwareEventConsumerBuilder(producer, lastMessageConsumer);
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}