using System;

using ParsecCore.EitherNS;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which consumes a single character and succeeds if the character passes a predicate
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
                    return Either.Error<ParseError, char>(new ParseError("Unexpected end of file, character expected", readPosition));
                }

                char c = input.Read();
                if (!predicate(c))
                {
                    return Either.Error<ParseError, char>(new ParseError($"character '{c}' does not conform, {predicateDescription} exprected", readPosition));
                }

                return Either.Result<ParseError, char>(c);
            };
        }
    }
}
