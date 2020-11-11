using System.Collections.Generic;

namespace Framework
{
    public sealed class PartitionName : ValueObject
    {
        private readonly string _name;

        private PartitionName(string name)
        {
            _name = name;
        }

        public static PartitionName Of(string name) => new PartitionName(name);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _name;
        }
        
        public override string ToString() => _name;

        public static implicit operator string(PartitionName partitionName) => partitionName.ToString();
    }
}