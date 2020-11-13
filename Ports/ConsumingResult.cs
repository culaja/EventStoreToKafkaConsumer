using System.Collections.Generic;
using Framework;

namespace Ports
{
    public sealed class ConsumingResult : ValueObject
    {
        public EventPosition EventPosition { get; }
        public Optional<string> OptionalFailureReason { get; }
        public bool IsSuccess => OptionalFailureReason.HasNoValue;

        public ConsumingResult(
            EventPosition eventPosition,
            Optional<string> optionalFailureReason)
        {
            EventPosition = eventPosition;
            OptionalFailureReason = optionalFailureReason;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return EventPosition;
            yield return OptionalFailureReason;
        }

        public override string ToString() => 
            $"{nameof(EventPosition)}: {EventPosition}, {nameof(OptionalFailureReason)}: {OptionalFailureReason}";
    }
}