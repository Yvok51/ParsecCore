using JSONtoXML.XMLValues;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JSONtoXML
{
    internal class PrintXML
    {
        public static void Print(XMLNode node, TextWriter writer)
        {
            var xmlDeclaration = $"<?xml version=\"1.0\" encoding=\"{writer.Encoding.WebName}\"?>";
            writer.Write(xmlDeclaration);
            writer.WriteLine(node.Accept(new XMLtoString(), 0));
            writer.Flush();
        }

        private class XMLtoString : IXMLVisitor<string, int>
        {
            public XMLtoString(int spacesPerIndent=4)
            {
                _spacesPerIndent = spacesPerIndent;
            }

            public string ElementVisit(XMLElementNode element, int indent)
            {
                return GetIndentation(indent, _spacesPerIndent)
                    + $"<{element.Tag}{AttributesToString(element.Attributes)}>\n"
                    + string.Concat(element.ChildNodes.Select(node => element.Accept(this, indent + 1)))
                    + $"</{element.Tag}>";
            }

            public string TextVisit(XMLTextNode text, int indent)
            {
                return GetIndentation(indent, _spacesPerIndent) + text.Content + '\n';
            }

            private static string GetIndentation(int indent, int spacesPerIndent)
            {
                return new string(' ', indent * spacesPerIndent);
            }

            private static string AttributesToString(Dictionary<string, string> attributes)
            {
                if (attributes is null)
                {
                    return string.Empty;
                }

                StringBuilder builder = new StringBuilder();
                foreach (var (key, value) in attributes)
                {
                    builder.Append($" {key}=\"{value}\"");
                }

                return builder.ToString();
            }

            private readonly int _spacesPerIndent;
        }
    }
}
