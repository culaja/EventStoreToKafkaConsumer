using System.Collections.Generic;

namespace Framework
{
    public sealed class EventPosition : ValueObject
    {
        private readonly ulong _position;

        private EventPosition(ulong position)
        {
            _position = position;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _position;
        }

        public static EventPosition Of(ulong position) => new EventPosition(position);
        
        public override string ToString() => _position.ToString();

        public static implicit operator ulong(EventPosition eventPosition) => eventPosition._position;
    }
}