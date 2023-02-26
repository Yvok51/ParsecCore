
using JSONtoXML.XMLValues;

namespace JSONtoXML
{
    internal sealed class XMLTextNode : XMLNode
    {
        public string Content { get; init; }

        public override T Accept<T, A>(IXMLVisitor<T, A> visitor, A arg)
        {
            return visitor.TextVisit(this, arg);
        }
    }
}
