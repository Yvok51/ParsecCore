namespace JSONParser
{
    class NullValue : JsonValue
    {
        public override string ToString() => "null";

        public override bool Equals(object obj) => obj is NullValue;
    }
}
