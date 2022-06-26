using System;

using ParsecCore.EitherNS;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which succeeds if the character passes a predicate.
    /// It consumes the read character only if the predicate is successfull
    /// </summary>
    class SatisfyParser
    {
        public static Parser<char> Parser(Predicate<char> predicate, string predicateDescription)
        {
            return (input) =>
            {
                var readPosition = input.Position;
                if (input.EndOfInput)
                {
                    return Either.Error<ParseError, char>(new ParseError("Unexpected end of file encountered", input.Position));
                }

                char read = input.Peek();

                if (!predicate(read))
                {
                    return Either.Error<ParseError, char>(new ParseError($"character '{read}' does not conform, {predicateDescription} exprected", readPosition));
                }

                input.Read();

                return Either.Result<ParseError, char>(read);
            };
        }
    }
}
