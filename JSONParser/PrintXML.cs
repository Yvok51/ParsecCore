﻿using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JSONtoXML
{
    class PrintXML
    {
        public static void Print(XMLNode node, TextWriter writer)
        {
            var xmlDeclaration = $"<?xml version=\"1.0\" encoding=\"{writer.Encoding.WebName}\"?>";
            writer.Write(xmlDeclaration);
            PrintImpl(node, writer, 0);
            writer.WriteLine();
            writer.Flush();
        }

        private static void PrintImpl(XMLNode node, TextWriter writer, int indentation)
        {
            if (node is XMLTextNode textNode)
            {
                PrintImpl(textNode, writer, indentation);
            }
            else if (node is XMLElementNode elNode)
            {
                PrintImpl(elNode, writer, indentation);
            }
        }

        private static void PrintImpl(XMLElementNode node, TextWriter writer, int indentation)
        {
            writer.WriteLine();
            writer.Write(IndentationToString(indentation));
            writer.Write($"<{node.Tag}{AttributesToString(node.Attributes)}");

            if (node.ChildNodes.Count == 0)
            {
                writer.Write("/>");
                return;
            }
            if (node.ChildNodes.Count == 1 && node.ChildNodes[0] is XMLTextNode textNode)
            {
                writer.Write(">");
                PrintImpl(textNode, writer, 0);
                writer.Write($"</{node.Tag}>");
                return;
            }

            writer.Write(">");

            foreach (var child in node.ChildNodes)
            {
                PrintImpl(child, writer, indentation + 1);
            }
            writer.WriteLine();
            writer.Write(IndentationToString(indentation));
            writer.Write($"</{node.Tag}>");
        }

        private static void PrintImpl(XMLTextNode node, TextWriter writer, int indentation)
        {
            writer.Write(IndentationToString(indentation));
            writer.Write(node.Content);
        }

        private static string singleIndentation = "  ";

        private static string IndentationToString(int indentation)
        {
            StringBuilder builder = new StringBuilder(indentation * singleIndentation.Length);
            for (int i = 0; i < indentation; i++)
            {
                builder.Append(singleIndentation);
            }
            return builder.ToString();
        }

        private static void WriteIndentation(TextWriter writer, int indentation)
        {
            for (int i = 0; i < indentation; i++)
            {
                writer.Write(indentation);
            }
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
    }
}
