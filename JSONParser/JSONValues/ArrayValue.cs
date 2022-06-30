using System.Collections.Generic;
using System.Text;

namespace JSONtoXML
{
    public class ArrayValue : JsonValue
    {
        private List<JsonValue> _values;

        public ArrayValue(IEnumerable<JsonValue> list)
        {
            _values = new List<JsonValue>(list);
        }

        public List<JsonValue> Value { get => _values; }

        public override string ToString() 
        {
            StringBuilder builder = new StringBuilder();
            string seperator = ",";
            builder.Append("[");
            foreach (var val in _values)
            {
                builder.Append(" ");
                builder.Append(val.ToString());
                builder.Append(seperator);
            }
            builder.Remove(builder.Length - seperator.Length, seperator.Length);
            builder.Append(" ]");
            return builder.ToString();
        }

        public override int GetHashCode() => _values.GetHashCode();

        public override bool Equals(object obj) => obj is ArrayValue value && Equals(value);

        public bool Equals(ArrayValue other) => !(other is null) && Value == other.Value;
    }
}
