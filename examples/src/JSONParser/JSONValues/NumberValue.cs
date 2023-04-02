using JSONtoXML.JSONValues;

namespace JSONtoXML
{
    internal sealed class NumberValue : JsonValue
    {
        private double _value;

        public NumberValue(double value)
        {
            _value = value;
        }

        public override T Accept<T, A>(IJsonVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitNumber(this, arg);
        }

        public double Value { get => _value; }

        public override string ToString() => _value.ToString();

        public override int GetHashCode() => _value.GetHashCode();

        public override bool Equals(object obj) => obj is NumberValue value && Equals(value);

        public bool Equals(NumberValue other) => !(other is null) && Value == other.Value;
    }
}
