using ParsecCore.Input;
using ParsecCore.EitherNS;

namespace ParsecCore.Parsers
{
    public class CharParser : IParser<char>
    {
        public CharParser(char c)
        {
            _charParser = new SatisfyParser(i => i == c, $"character {c}");
        }

        public IEither<ParseError, char> Parse(IParserInput input)
        {
            return _charParser.Parse(input);
        }

        private readonly IParser<char> _charParser;
    }
}
