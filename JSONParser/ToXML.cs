using System.Collections.Generic;

namespace JSONtoXML
{
    class ToXML
    {
        public static XMLNode ConvertJSON(JsonValue jsonValue)
        {
            return ConvertImpl(jsonValue, "root");
        }

        private static XMLNode ConvertImpl(JsonValue jsonValue, string tag, Dictionary<string, string> attributes = null)
        {
            var childNodes = jsonValue switch
            {
                ObjectValue value => Convert(value, tag, attributes),
                ArrayValue value => Convert(value, tag, attributes),
                StringValue value => Convert(value),
                NumberValue value => Convert(value),
                BoolValue value => Convert(value),
                NullValue value => Convert(value),
                _ => Help.ListOf(new XMLTextNode() { Content = "null" }) // Remove?
            };

            return new XMLElementNode()
            {
                Tag = tag,
                Attributes = attributes,
                ChildNodes = childNodes
            };
        }

        private static IReadOnlyList<XMLNode> Convert(ObjectValue objectValue, string tag, Dictionary<string, string> attributes = null)
        {
            List<XMLNode> childNodes = new List<XMLNode>();
            foreach (var (key, value) in objectValue.Value)
            {
                childNodes.Add(ConvertImpl(value, key.Value, null));
            }

            return childNodes;
        }

        private static IReadOnlyList<XMLNode> Convert(ArrayValue arrayValue, string tag, Dictionary<string, string> attributes = null)
        {
            List<XMLNode> nodes = new List<XMLNode>();
            foreach (var value in arrayValue.Value)
            {
                nodes.Add(ConvertImpl(value, tag, attributes));
            }

            return nodes;
        }

        private static IReadOnlyList<XMLNode> Convert(StringValue stringValue) =>
            Help.ListOf(new XMLTextNode() { Content = stringValue.Value });

        private static IReadOnlyList<XMLNode> Convert(BoolValue boolValue) =>
            Help.ListOf(new XMLTextNode() { Content = boolValue.Value ? "true" : "false" });

        private static IReadOnlyList<XMLNode> Convert(NullValue nullValue) =>
            Help.ListOf(new XMLTextNode() { Content = "null" });

        private static IReadOnlyList<XMLNode> Convert(NumberValue numberValue) =>
            Help.ListOf(new XMLTextNode() { Content = numberValue.Value.ToString() });
    }
}
