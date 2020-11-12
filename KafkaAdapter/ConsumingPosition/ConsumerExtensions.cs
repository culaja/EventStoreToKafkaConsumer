using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Framework;
using static Confluent.Kafka.ErrorCode;

namespace KafkaAdapter.ConsumingPosition
{
    internal static class ConsumerExtensions
    {
        public static IReadOnlyList<EventEnvelope> ConsumeLastEventEnvelopes(this IConsumer<string, string> consumer)
        {
            var list = new List<EventEnvelope>();
            while (true)
            {
                try
                {
                    Optional<ConsumeResult<string, string>> optionalConsumeResult =
                        consumer.Consume(TimeSpan.FromSeconds(5));
                    if (optionalConsumeResult.HasNoValue)
                    {
                        break;
                    }

                    var eventEnvelope = EventEnvelopeDto.Deserialize(optionalConsumeResult.Value.Message.Value)
                        .ToDomain();
                    list.Add(eventEnvelope);
                }
                catch (ConsumeException ex)
                {
                    switch (ex)
                    {
                        case var e when e.Error.Code == UnknownTopicOrPart:
                            return list;
                        default:
                            throw;
                    }
                }
            }

            return list;
        }
    }
}