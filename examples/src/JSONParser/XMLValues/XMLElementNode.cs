using JSONtoXML.XMLValues;
using System.Collections.Generic;

namespace JSONtoXML
{
    internal sealed class XMLElementNode : XMLNode
    {
        public string Tag { get; init; }
        public Dictionary<string, string> Attributes { get; init; }
        public IReadOnlyList<XMLNode> ChildNodes { get; init; }

        public override T Accept<T, A>(IXMLVisitor<T, A> visitor, A arg)
        {
            return visitor.ElementVisit(this, arg);
        }
    }
}
