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

        private LastMessageConsumer(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public static LastMessageConsumer For(
            string connectionString) => new LastMessageConsumer(connectionString);

        public IReadOnlyList<EventEnvelope> ConsumeLastEventEnvelopesFor(TopicName topicName)
        {
            using var consumer = BuildConsumer();
            consumer.Subscribe(topicName);
            return consumer.ConsumeLastEventEnvelopes();
        }

        private IConsumer<string, string> BuildConsumer()
        {
            var consumerConfig = new ConsumerConfig
            { 
                GroupId = Guid.NewGuid().ToString(),
                BootstrapServers = _connectionString
            };
            var builder = new ConsumerBuilder<string, string>(consumerConfig)
                .SetPartitionsAssignedHandler(OnPartitionAssigned);

            return builder.Build();
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
    }
}