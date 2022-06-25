using System.Collections.Generic;

namespace JSONParser
{
    class ObjectValue : JsonValue
    {
        private Dictionary<StringValue, JsonValue> _values;

        public ObjectValue(IEnumerable<ObjectKeyValuePair> members)
        {
            _values = new Dictionary<StringValue, JsonValue>();
            foreach (var pair in members)
            {
                _values.Add(pair.Key, pair.Value);
            }
        }

        public Dictionary<StringValue, JsonValue> Value { get => _values; }

        public override string ToString() => _values.ToString();

        public override int GetHashCode() => _values.GetHashCode();

        public override bool Equals(object obj) => obj is ObjectValue value && Equals(value);

        public bool Equals(ObjectValue other) => !(other is null) && Value == other.Value;
    }
}
