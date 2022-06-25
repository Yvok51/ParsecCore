using System.Collections.Generic;

namespace JSONParser
{
    class ArrayValue : JsonValue
    {
        private List<JsonValue> _values;

        public ArrayValue(IEnumerable<JsonValue> list)
        {
            _values = new List<JsonValue>(list);
        }

        public List<JsonValue> Value { get => _values; }

        public override string ToString() => _values.ToString();

        public override int GetHashCode() => _values.GetHashCode();

        public override bool Equals(object obj) => obj is ArrayValue value && Equals(value);

        public bool Equals(ArrayValue other) => !(other is null) && Value == other.Value;
    }
}
