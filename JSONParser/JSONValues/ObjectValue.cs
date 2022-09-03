using System.Collections.Generic;
using System.Text;

namespace JSONtoXML
{
    internal sealed class ObjectValue : JsonValue
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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string seperator = ",";
            builder.Append("{");
            foreach (var val in _values)
            {
                builder.Append($" {val.Key}: {val.Value}");
                builder.Append(seperator);
            }
            if (builder.Length > "{".Length)
            {
                builder.Remove(builder.Length - seperator.Length, seperator.Length);
            }
            builder.Append(" }");
            return builder.ToString();
        }

        public override int GetHashCode() =>
            _values.GetHashCode();

        public override bool Equals(object obj) =>
            obj is ObjectValue value && Equals(value);

        public bool Equals(ObjectValue other) =>
            !(other is null) && Value == other.Value;

        public bool MemberwiseEquals(ObjectValue other) =>
            !(other is null) && Value.MemberwiseEquals(other.Value);
    }
}
