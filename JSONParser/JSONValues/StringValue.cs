namespace JSONParser
{
    class StringValue : JsonValue
    {
        private string _value;

        public StringValue(string value)
        {
            _value = value;
        }

        public string Value { get => _value; }

        public override string ToString() => $"\"{_value}\"";

        public override int GetHashCode() => _value.GetHashCode();

        public override bool Equals(object obj) => obj is StringValue value && Equals(value);

        public bool Equals(StringValue other) => !(other is null) && Value == other.Value;
    }
}
