using JSONtoXML.JSONValues;
using System.Collections.Generic;
using System.Linq;

namespace JSONtoXML
{
    internal class ToXML
    {
        public static XMLNode ConvertJSON(JsonValue jsonValue)
        {
            return jsonValue.Accept(new JsonToXmlVisitor(), "root");
        }

        private class JsonToXmlVisitor : IJsonVisitor<XMLNode, string>
        {
            public XMLNode VisitArray(ArrayValue value, string tag)
            {
                return new XMLElementNode()
                {
                    Tag = tag,
                    Attributes = null,
                    ChildNodes = value.Value.Select(json => json.Accept(this, "Item")).ToList()
                };
            }

            public XMLNode VisitBool(BoolValue value, string tag)
            {
                return new XMLElementNode()
                {
                    Tag = tag,
                    Attributes = null,
                    ChildNodes = new List<XMLNode>() { new XMLTextNode() { Content = value.Value ? "true" : "else" } }
                };
            }

            public XMLNode VisitNull(NullValue value, string tag)
            {
                return new XMLElementNode()
                {
                    Tag = tag,
                    Attributes = null,
                    ChildNodes = new List<XMLNode>() { new XMLTextNode() { Content = "null" } }
                };
            }

            public XMLNode VisitNumber(NumberValue value, string tag)
            {
                return new XMLElementNode()
                {
                    Tag = tag,
                    Attributes = null,
                    ChildNodes = new List<XMLNode>() { new XMLTextNode() { Content = value.Value.ToString() } }
                };
            }

            public XMLNode VisitObject(ObjectValue value, string tag)
            {
                return new XMLElementNode()
                {
                    Tag = tag,
                    Attributes = null,
                    ChildNodes = value.Value.Select(pair => pair.Value.Accept(this, pair.Key.Value)).ToList()
                };
            }

            public XMLNode VisitString(StringValue value, string tag)
            {
                return new XMLElementNode()
                {
                    Tag = tag,
                    Attributes = null,
                    ChildNodes = new List<XMLNode>() { new XMLTextNode() { Content = value.Value } }
                };
            }
        }
    }
}
