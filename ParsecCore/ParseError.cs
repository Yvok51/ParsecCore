using System.Text;
using System.Collections.Generic;

using ParsecCore.Input;

namespace ParsecCore
{
    /// <summary>
    /// Represents an error which occured during parsing
    /// </summary>
    public struct ParseError
    {
        public ParseError(ErrorMessage error, Position position)
        {
            Position = position;
            Errors = new List<ErrorMessage>();
            Errors.Add(error);
        }

        public ParseError(List<ErrorMessage> errors, Position position)
        {
            Position = position;
            Errors = errors;
        }

        public Position Position { get; init; }
        public List<ErrorMessage> Errors { get; init; }

        /// <summary>
        /// Creates a new ParseError with same position but a different message
        /// </summary>
        /// <param name="errorMsg"> The new error message </param>
        /// <returns> ParseError with a new error message </returns>
        public ParseError WithErrorMessage(ErrorMessage errorMsg) =>
            new ParseError(errorMsg, Position);

        /// <summary>
        /// Creates a new ParseError with same position but a different messages
        /// </summary>
        /// <param name="errorMsg"> The new error message </param>
        /// <returns> ParseError with a new error message </returns>
        public ParseError WithErrorMessages(List<ErrorMessage> errorMsgs) =>
            new ParseError(errorMsgs, Position);

        /// <summary>
        /// Combine two errors.
        /// We take the error that is more exact - that is the one which parsed more of the input 
        /// (its Position is more advanced).
        /// If both errors parsed the same amount of the input then we combine their error messages together
        /// </summary>
        /// <param name="secondError"> The error to combine with </param>
        /// <returns> The combined error </returns>
        public ParseError Combine(ParseError secondError)
        {
            if (Position < secondError.Position)
            {
                return secondError;
            }
            else if (secondError.Position < Position)
            {
                return this;
            }
            else
            {
                List<ErrorMessage> concatErrors = new(Errors.Count + secondError.Errors.Count);
                concatErrors.AddRange(Errors);
                concatErrors.AddRange(secondError.Errors);

                return new ParseError(concatErrors, Position);
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"(line {Position.Line}, column {Position.Column}):\n");
            if (stringBuilder.Length == 0)
            {
                return stringBuilder.Append("  Unknown parsing error").ToString();
            }

            return AddErrorMessages(stringBuilder, Errors).ToString();
        }

        private StringBuilder AddErrorMessages(StringBuilder stringBuilder, List<ErrorMessage> errorMessages)
        {
            stringBuilder.Append("     ");
            stringBuilder.Append(errorMessages[0]);
            stringBuilder.Append('\n');

            for (int i = 1; i < errorMessages.Count; ++i)
            {
                stringBuilder.Append("  or ");
                stringBuilder.Append(errorMessages[i]);
                stringBuilder.Append('\n');
            }

            return stringBuilder.Remove(stringBuilder.Length - 1, 1);
        }
    }
}
