using JSONtoXML.JSONValues;

namespace JSONtoXML
{
    internal sealed class BoolValue : JsonValue
    {
        private bool _value;

        public BoolValue(bool value)
        {
            _value = value;
        }

        public override T Accept<T, A>(IJsonVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitBool(this, arg);
        }

        public bool Value { get => _value; }

        public override string ToString() => _value.ToString();

        public override int GetHashCode() => _value.GetHashCode();

        public override bool Equals(object obj) => obj is BoolValue value && Equals(value);

        public bool Equals(BoolValue other) => !(other is null) && Value == other.Value;
    }
}
