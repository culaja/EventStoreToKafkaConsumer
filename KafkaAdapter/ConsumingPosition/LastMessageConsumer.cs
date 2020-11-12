using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Framework;

namespace KafkaAdapter.ConsumingPosition
{
    internal sealed class LastMessageConsumer
    {
        private readonly string _connectionString;
        private readonly string _adminConsumerGroupId;

        private LastMessageConsumer(string connectionString, string adminConsumerGroupId)
        {
            _connectionString = connectionString;
            _adminConsumerGroupId = adminConsumerGroupId;
        }
        
        public static LastMessageConsumer For(string connectionString, string adminConsumerGroupId) => 
            new LastMessageConsumer(connectionString, adminConsumerGroupId);

        public IReadOnlyList<EventEnvelope> ConsumeLastEventEnvelopesFor(TopicName topicName)
        {
            using var consumer = BuildConsumer();
            AssignToLastMessageInAllPartitions(consumer, GetPartitionsFor(topicName));
            
            return consumer.ConsumeLastEventEnvelopes();
        }

        private IReadOnlyList<TopicPartition> GetPartitionsFor(TopicName topicName)
        {
            var config = new AdminClientConfig {BootstrapServers = _connectionString};
            using var adminClient = new AdminClientBuilder(config).Build();
            return adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(5))
                .Topics.SelectMany(t =>
                    t.Partitions.Select(p =>
                        new TopicPartition(t.Topic, p.PartitionId)))
                .ToList();
        }
        
        private static void AssignToLastMessageInAllPartitions(
            IConsumer<string, string> consumer, 
            IReadOnlyList<TopicPartition> topicPartitions)
        {
            foreach (var topicPartition in topicPartitions)
            {
                var watermarkOffsets = consumer.QueryWatermarkOffsets(topicPartition, TimeSpan.FromSeconds(5));
                var lastOffset = watermarkOffsets.High > 0 ? new Offset(watermarkOffsets.High - 1) : watermarkOffsets.High;
                consumer.Assign(new TopicPartitionOffset(topicPartition.Topic, topicPartition.Partition, lastOffset));
            }
        }

        private IConsumer<string, string> BuildConsumer()
        {
            var consumerConfig = new ConsumerConfig
            { 
                GroupId = _adminConsumerGroupId,
                BootstrapServers = _connectionString
            };
            var builder = new ConsumerBuilder<string, string>(consumerConfig);

            return builder.Build();
        }
    }
}