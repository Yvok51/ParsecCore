using JSONtoXML.JSONValues;

namespace JSONtoXML
{
    internal abstract class JsonValue
    {
        public abstract T Accept<T, A>(IJsonVisitor<T, A> visitor, A arg);
    }
}
