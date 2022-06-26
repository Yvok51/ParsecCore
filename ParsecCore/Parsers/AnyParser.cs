using ParsecCore.EitherNS;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser any character
    /// Fails if we are at the end of the input
    /// </summary>
    class AnyParser
    {
        public static Parser<char> Parser()
        {
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return new Error<ParseError, char>(new ParseError("Unexpected end of file encountered", input.Position));
                }

                char matched = input.Read();
                return new Result<ParseError, char>(matched);
            };
        }
    }
}
