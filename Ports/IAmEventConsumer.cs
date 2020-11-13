using Framework;

namespace Ports
{
    public delegate void ConsumingResultDelegate(ConsumingResult consumingResult);
    
    public interface IAmEventConsumer
    {
        void RegisterOnConsumingResults(ConsumingResultDelegate consumingResultDelegate);
        
        void Consume(EventEnvelope eventEnvelopes);
    }
}