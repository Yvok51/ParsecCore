using ParsecCore.EitherNS;
using ParsecCore.Help;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which parses the end of the file.
    /// Fails if any character is present
    /// </summary>
    class EOFParser
    {
        public static Parser<None, TInputToken> Parser<TInputToken>()
        {
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return new ResultValue<ParseError, None>(new None());
                }

                return new ErrorValue<ParseError, None>(new ParseError("Unexpected char encountered, expected EOF", input.Position));
            };
        }
    }
}
