using JSONtoXML.JSONValues;

namespace JSONtoXML
{
    internal sealed class NullValue : JsonValue
    {
        public override T Accept<T, A>(IJsonVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitString(this, arg);
        }

        public override string ToString() => "null";

        public override bool Equals(object obj) => obj is NullValue;

        public override int GetHashCode() => base.GetHashCode();
    }
}
