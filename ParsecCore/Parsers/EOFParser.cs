using ParsecCore.EitherNS;
using ParsecCore.Help;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which parses the end of the file.
    /// Fails if any character is present
    /// </summary>
    internal class EOFParser
    {
        public static Parser<None, TInputToken> Parser<TInputToken>()
        {
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return new ResultValue<ParseError, None>(None.Instance);
                }

                return Either.Error<ParseError, None>(
                    new ParseError(
                        expected: "end of file",
                        encoutered: "character",
                        input.Position
                    )
                );
            };
        }
    }
}
