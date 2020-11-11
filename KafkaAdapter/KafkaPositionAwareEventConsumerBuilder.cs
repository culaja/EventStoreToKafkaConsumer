using System;
using System.Collections.Concurrent;
using Confluent.Kafka;
using Ports;
using Partitioner = Confluent.Kafka.Partitioner;

namespace KafkaAdapter
{
    public sealed class KafkaPositionAwareEventConsumerBuilder : IDisposable
    {
        private readonly IProducer<string, string> _producer;

        private KafkaPositionAwareEventConsumerBuilder(IProducer<string, string> producer)
        {
            _producer = producer;
        }

        public IAmPositionAwareEventConsumer PositionAwareEventConsumer => new KafkaPositionAwareEventConsumer(_producer);
        
        public static KafkaPositionAwareEventConsumerBuilder NewUsing(string kafkaConnectionString)
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092"};
            var producer = new ProducerBuilder<string, string>(config).Build();
            
            return new KafkaPositionAwareEventConsumerBuilder(producer);
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}