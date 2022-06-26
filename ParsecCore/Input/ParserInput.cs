using System.IO;
using System.Text;

namespace ParsecCore.Input
{
    public class ParserInput
    {
        public static IParserInput Create(string inputString) => new StringParserInput(inputString);

        public static IParserInput Create(Stream stream, Encoding encoding) => new StreamParserInput(stream, encoding);
    }
}
