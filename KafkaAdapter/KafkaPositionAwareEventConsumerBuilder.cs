using System;
using System.Collections.Generic;
using System.Linq;
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
                BootstrapServers = kafkaConnectionString
            };
            var builder = new ConsumerBuilder<string, string>(consumerConfig)
                .SetPartitionsAssignedHandler(OnPartitionAssigned);
            
            return new KafkaPositionAwareEventConsumerBuilder(producer, builder);
        }

        private static IReadOnlyList<TopicPartitionOffset> OnPartitionAssigned(
            IConsumer<string, string> consumer,
            List<TopicPartition> topicPartitions)
        {
            return topicPartitions
                .Select(tp =>
                {
                    var watermarkOffsets = consumer.QueryWatermarkOffsets(tp, TimeSpan.FromSeconds(5));
                    var lastOffset = watermarkOffsets.High > 0 ? new Offset(watermarkOffsets.High - 1) : watermarkOffsets.High;
                    return new TopicPartitionOffset(tp.Topic, tp.Partition, lastOffset);
                })
                .ToList();
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}