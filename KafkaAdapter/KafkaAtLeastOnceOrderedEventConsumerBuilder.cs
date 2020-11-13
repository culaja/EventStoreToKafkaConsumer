using System;
using Confluent.Kafka;
using Ports;

namespace KafkaAdapter
{
    public sealed class KafkaAtLeastOnceOrderedEventConsumerBuilder : IDisposable
    {
        private readonly IProducer<string, string> _producer;

        private KafkaAtLeastOnceOrderedEventConsumerBuilder(IProducer<string, string> producer)
        {
            _producer = producer;
            Consumer = new KafkaAtLeastOnceOrderedEventConsumer(_producer);
        }

        public IAmAtLeastOnceOrderedEventConsumer Consumer { get; }
        
        public static KafkaAtLeastOnceOrderedEventConsumerBuilder NewUsing(string kafkaConnectionString)
        {
            var producerConfig = new ProducerConfig { BootstrapServers = kafkaConnectionString };
            var producer = new ProducerBuilder<string, string>(producerConfig).Build();
            
            return new KafkaAtLeastOnceOrderedEventConsumerBuilder(producer);
        }

        public void Dispose()
        {
            _producer?.Flush();
            _producer?.Dispose();
        }
    }
}