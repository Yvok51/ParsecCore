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
                var result = AnyParser.Parser()(input);
                if (result.HasLeft) 
                {
                    return result;
                }

                if (!predicate(result.Right))
                {
                    return Either.Error<ParseError, char>(new ParseError($"character '{result.Right}' does not conform, {predicateDescription} exprected", readPosition));
                }

                return result;
            };
        }
    }
}
