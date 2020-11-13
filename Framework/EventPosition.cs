using System.Collections.Generic;

namespace Framework
{
    public sealed class EventPosition : ValueObject
    {
        private readonly long[] _position;

        private EventPosition(params long[] position)
        {
            _position = position;
        }
        
        public static readonly EventPosition Beginning = new EventPosition(0L, 0L);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var item in _position) yield return item;
        }

        public static EventPosition Of(params long[] position) => new EventPosition(position);

        public override string ToString() => string.Join(".", _position);

        public static implicit operator long[](EventPosition eventPosition) => eventPosition._position;
    }
}