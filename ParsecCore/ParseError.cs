using System.Text;
using System.Collections.Generic;
using System.Linq;

using ParsecCore.Input;
using ParsecCore.Help;

namespace ParsecCore
{
    /// <summary>
    /// Represents an error which occured during parsing
    /// </summary>
    public struct ParseError
    {
        private ParseError(
            Position position,
            List<GenericMessage> genericMessages,
            List<ExpectedMessage> expectedMessages,
            List<EncounteredMessage> encounteredMessages
        )
        {
            Position = position;
            _genericErrors = genericMessages;
            _expectedErrors = expectedMessages;
            _encounteredErrors = encounteredMessages;
        }

        public ParseError(string errorMessage, Position position)
        {
            Position = position;
            _genericErrors = new();
            _expectedErrors = new();
            _encounteredErrors = new();
            _genericErrors.Add(new GenericMessage(errorMessage));
        }

        public ParseError(string expected, string encoutered, Position position)
        {
            Position = position;
            _genericErrors = new();
            _expectedErrors = new();
            _encounteredErrors = new();

            _expectedErrors.Add(new ExpectedMessage(expected));
            _encounteredErrors.Add(new EncounteredMessage(encoutered));
        }

        public Position Position { get; init; }

        private List<GenericMessage> _genericErrors;
        private List<ExpectedMessage> _expectedErrors;
        private List<EncounteredMessage> _encounteredErrors;

        /// <summary>
        /// Creates a new ParseError with same position but a different message
        /// </summary>
        /// <param name="errorMsg"> The new error message </param>
        /// <returns> ParseError with a new error message </returns>
        public ParseError WithExpectedMessage(string expected)
        {
            List<ExpectedMessage> expectedErrors = new();
            expectedErrors.Add(new ExpectedMessage(expected));

            return new ParseError(Position, new(_genericErrors), expectedErrors, new(_encounteredErrors));
        }

        /// <summary>
        /// Combine two errors.
        /// We take the error that is more exact - that is the one which parsed more of the input 
        /// (its <see cref="Input.Position"/> is more advanced).
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
                List<EncounteredMessage> newEncountered = 
                    _encounteredErrors.Concat(secondError._encounteredErrors).Distinct().ToList();
                List<ExpectedMessage> newExpected = 
                    _expectedErrors.Concat(secondError._expectedErrors).Distinct().ToList();
                List<GenericMessage> newGeneric = 
                    _genericErrors.Concat(secondError._genericErrors).Distinct().ToList();

                return new ParseError(Position, newGeneric, newExpected, newEncountered);
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"(line {Position.Line}, column {Position.Column}):\n");
            if (_expectedErrors.Count == 0 && _encounteredErrors.Count == 0 && _genericErrors.Count == 0)
            {
                return stringBuilder.Append("  Unknown parsing error").ToString();
            }

            AddErrorMessages(stringBuilder, _genericErrors);
            AddErrorMessages(stringBuilder, _encounteredErrors);
            AddErrorMessages(stringBuilder, _expectedErrors);

            return stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString(); // remove last \n
        }

        private StringBuilder AddErrorMessages<T>(StringBuilder stringBuilder, List<T> errorMessages)
        {
            if (errorMessages.Count == 0)
            {
                return stringBuilder;
            }

            stringBuilder.Append("     ");
            stringBuilder.Append(errorMessages[0]);
            stringBuilder.Append('\n');

            for (int i = 1; i < errorMessages.Count; ++i)
            {
                stringBuilder.Append("  or ");
                stringBuilder.Append(errorMessages[i]);
                stringBuilder.Append('\n');
            }

            return stringBuilder;
        }
    }
}
