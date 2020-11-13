using Framework;

namespace Ports
{
    public delegate void ConsumingResultDelegate(ConsumingResult consumingResult);
    
    public interface IAmAtLeastOnceOrderedEventConsumer
    {
        void RegisterOnConsumingResults(ConsumingResultDelegate consumingResultDelegate);
        
        void Consume(EventEnvelope eventEnvelopes);
    }
}