using ParsecCore.Help;
using ParsecCore.EitherNS;
using ParsecCore.Input;

namespace ParsecCore.Parsers
{
    /// <summary>
    /// Parser which parses the end of the file.
    /// Fails if any character is present
    /// </summary>
    class EOFParser : IParser<None>
    {
        public IEither<ParseError, None> Parse(IParserInput input)
        {
            if (input.EndOfInput)
            {
                return new Result<ParseError, None>(new None());
            }

            return new Error<ParseError, None>(new ParseError("Unexpected char encountered, expected EOF", input.Position));
        }
    }
}
