using ParsecCore.Help;
using ParsecCore.EitherNS;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which parses the end of the file.
    /// Fails if any character is present
    /// </summary>
    class EOFParser
    {
        public static Parser<None> Parser()
        {
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return new Result<ParseError, None>(new None());
                }

                return new Error<ParseError, None>(new ParseError("Unexpected char encountered, expected EOF", input.Position));
            };
        }
    }
}
