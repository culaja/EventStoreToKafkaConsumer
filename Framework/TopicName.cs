using System.Collections.Generic;

namespace Framework
{
    public sealed class TopicName : ValueObject
    {
        private readonly string _name;

        private TopicName(string name)
        {
            _name = name;
        }

        public static TopicName Of(string name) => new TopicName(name);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _name;
        }

        public override string ToString() => _name;

        public static implicit operator string(TopicName topicName) => topicName.ToString();
    }
}