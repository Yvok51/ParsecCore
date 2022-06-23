using ParsecCore.Input;

namespace ParsecCore.Parsers
{
    class CharParser : IParser<char>
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
