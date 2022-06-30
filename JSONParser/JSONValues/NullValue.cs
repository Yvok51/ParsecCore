namespace JSONtoXML
{
    public class NullValue : JsonValue
    {
        public override string ToString() => "null";

        public override bool Equals(object obj) => obj is NullValue;

        public override int GetHashCode() => base.GetHashCode();
    }
}
