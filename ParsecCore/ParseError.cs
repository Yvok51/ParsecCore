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
        public ParseError(string error, Position position)
        {
            Position = position;
            Errors = new List<string>();
            Errors.Add(error);
        }

        public ParseError(List<string> errors, Position position)
        {
            Position = position;
            Errors = errors;
        }

        /// <summary>
        /// Creates a new ParseError with same position but a different message
        /// </summary>
        /// <param name="errorMsg"> The new error message </param>
        /// <returns> ParseError with a new error message </returns>
        public ParseError WithErrorMessage(string errorMsg) =>
            new ParseError(errorMsg, Position);

        /// <summary>
        /// Creates a new ParseError with same position but a different messages
        /// </summary>
        /// <param name="errorMsg"> The new error message </param>
        /// <returns> ParseError with a new error message </returns>
        public ParseError WithErrorMessages(List<string> errorMsgs) =>
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
                List<string> concatErrors = new List<string>(Errors.Count + secondError.Errors.Count);
                concatErrors.AddRange(Errors);
                concatErrors.AddRange(secondError.Errors);

                return new ParseError(concatErrors, Position);
            }
        }

        public Position Position { get; init; }

        public List<string> Errors { get; init; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string prefix = $"{Position.Line}:{Position.Column} - ";
            foreach (var errorMsg in Errors)
            {
                stringBuilder.Append(prefix);
                stringBuilder.Append(errorMsg);
                stringBuilder.Append('\n');
            }
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            return stringBuilder.ToString();
        }
    }
}
