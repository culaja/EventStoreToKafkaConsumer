using System.Linq;
using System.Threading.Tasks;
using Framework;
using KafkaAdapter;

namespace KafkaPositionAwareEventConsumerTestApp
{
    public sealed class TestEvent : IAmEvent
    {
        public static TestEvent New => new TestEvent();
    }
    
    public static class Program
    {
        public static async Task Main()
        {
            using var producerBuilder = KafkaPositionAwareEventConsumerBuilder.NewUsing("localhost:9092");

            var eventEnvelopes = Enumerable.Range(0, 10000)
                .Select(i => new EventEnvelope(
                    TopicName.Of("TestTopic"),
                    PartitionName.Of($"TestTopic.{i % 10}"),
                    EventPosition.Of((ulong)i),
                    TestEvent.New)).ToList();

            await producerBuilder.PositionAwareEventConsumer.Consume(eventEnvelopes);
        }
    }
}