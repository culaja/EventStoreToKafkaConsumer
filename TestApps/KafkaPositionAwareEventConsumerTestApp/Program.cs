using System;
using System.Linq;
using Framework;
using KafkaAdapter;
using Newtonsoft.Json;

namespace KafkaPositionAwareEventConsumerTestApp
{
    public sealed class TestEvent
    {
        public TestEvent(string someData, int someInt)
        {
            SomeData = someData;
            SomeInt = someInt;
        }

        public static TestEvent New => new TestEvent("Test", 6);
        
        public string SomeData { get; }
        
        public int SomeInt { get; }
    }
    
    public static class Program
    {
        private static readonly TopicName TestTopicName = TopicName.Of("TestTopic");
        
        public static void Main()
        {
            using var consumerBuilder = KafkaAtLeastOnceOrderedEventConsumerBuilder.NewUsing("localhost:9092");
            
            consumerBuilder.Consumer.RegisterOnConsumingResults(consumingReport => 
                Console.WriteLine(consumingReport.IsSuccess ? "." : "X"));

            var eventEnvelopes = Enumerable.Range(0, 10000)
                .Select(i => new EventEnvelope(
                    TestTopicName,
                    PartitioningKey.Of($"TestTopic.{i % 10}"),
                    EventPosition.Of((ulong)i),
                    JsonConvert.SerializeObject(TestEvent.New))).ToList();

            foreach (var eventEnvelope in eventEnvelopes)
            {
                consumerBuilder.Consumer.Consume(eventEnvelope);
            }
        }
    }
}