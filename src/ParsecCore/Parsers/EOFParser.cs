using ParsecCore.MaybeNS;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which parses the end of the file.
    /// Fails if any character is still present.
    /// </summary>
    internal class EOFParser
    {
        public static Parser<None, TInputToken> Parser<TInputToken>()
        {
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Result.Success(None.Instance, input);
                }

                return Result.Failure<None, TInputToken>(
                    new StandardError(input.Position, Maybe.Nothing<ErrorItem>(), EndOfFile.Instance),
                    input
                );
            };
        }
    }
}
