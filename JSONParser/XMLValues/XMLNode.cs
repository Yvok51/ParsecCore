
using JSONtoXML.XMLValues;

namespace JSONtoXML
{
    internal abstract class XMLNode
    {
        public abstract T Accept<T, A>(IXMLVisitor<T, A> visitor, A arg);
    }
}
