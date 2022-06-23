using ParsecCore.EitherNS;
using ParsecCore.Input;

namespace ParsecCore.Parsers
{
    /// <summary>
    /// Parser any character
    /// Fails if we are at the end of the input
    /// </summary>
    public class AnyParser : IParser<char>
    {
        public IEither<ParseError, char> Parse(IParserInput input)
        {
            if (input.EndOfInput)
            {
                return new Error<ParseError, char>(new ParseError("Unexpected end of file encountered", input.Position));
            }

            char matched = input.Read();
            return new Result<ParseError, char>(matched);
        }
    }
}
