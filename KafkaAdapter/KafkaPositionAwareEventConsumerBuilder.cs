using System;
using Confluent.Kafka;
using Ports;
using Partitioner = Confluent.Kafka.Partitioner;

namespace KafkaAdapter
{
    public sealed class KafkaPositionAwareEventConsumerBuilder : IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ConsumerBuilder<string, string> _consumerBuilder;

        private KafkaPositionAwareEventConsumerBuilder(
            IProducer<string, string> producer,
            ConsumerBuilder<string, string> consumerBuilder)
        {
            _producer = producer;
            _consumerBuilder = consumerBuilder;
        }

        public IAmPositionAwareEventConsumer PositionAwareEventConsumer => 
            new KafkaPositionAwareEventConsumer(_producer, _consumerBuilder);
        
        public static KafkaPositionAwareEventConsumerBuilder NewUsing(
            string kafkaConnectionString,
            string buildingKafkaTopicConsumerGroup = "buildingKafkaTopicConsumerGroup")
        {
            var producerConfig = new ProducerConfig { BootstrapServers = kafkaConnectionString, Partitioner = Partitioner.Consistent};
            var producer = new ProducerBuilder<string, string>(producerConfig).Build();
            
            var consumerConfig = new ConsumerConfig
            { 
                GroupId = buildingKafkaTopicConsumerGroup,
                BootstrapServers = kafkaConnectionString,
                AutoOffsetReset = AutoOffsetReset.Latest
            };
            
            return new KafkaPositionAwareEventConsumerBuilder(producer, new ConsumerBuilder<string, string>(consumerConfig));
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}