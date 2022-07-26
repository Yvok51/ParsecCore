using System.IO;
using System.Text;

namespace ParsecCore.Input
{
    public class ParserInput
    {
        public static IParserInput<char> Create(string inputString, int tabSize = 4) => new StringParserInput(inputString, tabSize);

        public static IParserInput<char> Create(Stream stream, Encoding encoding, int tabSize = 4) => new StreamParserInput(stream, encoding, tabSize);

        public static IParserInput<char> Create(StreamReader reader, int tabSize = 4) => new StreamParserInput(reader, tabSize);
    }
}
