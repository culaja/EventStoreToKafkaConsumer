using System.Collections.Generic;

namespace Framework
{
    public sealed class PartitioningKey : ValueObject
    {
        private readonly string _key;

        private PartitioningKey(string key)
        {
            _key = key;
        }

        public static PartitioningKey Of(string key) => new PartitioningKey(key);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _key;
        }
        
        public override string ToString() => _key;

        public static implicit operator string(PartitioningKey partitioningKey) => partitioningKey.ToString();
    }
}