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

        public Position Position { get; init; }

        public string Error { get; init; }

        public override string ToString()
        {
            return $"{Position.Line}:{Position.Column} - {Error}";
        }
    }
}
