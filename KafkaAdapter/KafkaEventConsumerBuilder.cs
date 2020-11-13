using System;
using Confluent.Kafka;
using Ports;

namespace KafkaAdapter
{
    public sealed class KafkaEventConsumerBuilder : IDisposable
    {
        private readonly IProducer<string, string> _producer;

        private KafkaEventConsumerBuilder(IProducer<string, string> producer)
        {
            _producer = producer;
            EventConsumer = new KafkaEventConsumer(_producer);
        }

        public IAmEventConsumer EventConsumer { get; }
        
        public static KafkaEventConsumerBuilder NewUsing(string kafkaConnectionString)
        {
            var producerConfig = new ProducerConfig { BootstrapServers = kafkaConnectionString };
            var producer = new ProducerBuilder<string, string>(producerConfig).Build();
            
            return new KafkaEventConsumerBuilder(producer);
        }

        public void Dispose()
        {
            _producer?.Flush();
            _producer?.Dispose();
        }
    }
}