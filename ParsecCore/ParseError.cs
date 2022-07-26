
using ParsecCore.Input;

namespace ParsecCore
{
    /// <summary>
    /// Represents an error which occured during parsing
    /// </summary>
    public struct ParseError
    {
        public ParseError(string error, Position position)
        {
            Position = position;
            Error = error;
        }

        /// <summary>
        /// Creates a new ParseError with same position but a different message
        /// </summary>
        /// <param name="errorMsg"> The new error message </param>
        /// <returns> ParseError with a new error message </returns>
        public ParseError WithErrorMessage(string errorMsg) =>
            new ParseError(errorMsg, Position);

        /// <summary>
        /// Combine two errors so that both of their error messages show.
        /// The position of the error is set to the minimum of the positions of the two errors
        /// </summary>
        /// <param name="secondError"> The error to combine with </param>
        /// <returns> The combined error </returns>
        public ParseError Combine(ParseError secondError)
        {
            return new ParseError(
                Error + "\n  or: " + secondError.Error,
                Position.Min(Position, secondError.Position)
            );
        }

        public Position Position { get; init; }

        public string Error { get; init; }

        public override string ToString()
        {
            return $"{Position.Line}:{Position.Column} - {Error}";
        }
    }
}
