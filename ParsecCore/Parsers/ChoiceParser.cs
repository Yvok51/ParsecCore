using ParsecCore.EitherNS;
using ParsecCore.Input;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore.ParsersHelp
{
    internal class ChoiceParser
    {
        public static Parser<T, TInputToken> Parser<T, TInputToken>(params Parser<T, TInputToken>[] parsers)
        {
            return Parser((IEnumerable<Parser<T, TInputToken>>)parsers);
        }

        public static Parser<T, TInputToken> Parser<T, TInputToken>(IEnumerable<Parser<T, TInputToken>> parsers)
        {
            return (input) =>
            {
                Position initialPosition = input.Position;
                List<IEither<ParseError, T>> parseResults = new();
                foreach (var parser in parsers)
                {
                    IEither<ParseError, T> currentParseResult = parser(input);
                    parseResults.Add(currentParseResult);
                    if (currentParseResult.IsResult)
                    {
                        return currentParseResult;
                    }
                    if (input.Position != initialPosition)
                    {
                        return parseResults.Aggregate((left, right) => left.CombineErrors(right));
                    }

                }
                if (parseResults.Count == 0)
                {
                    return Either.Error<ParseError, T>(new ParseError("No parser provided", initialPosition));
                }

                return parseResults.Aggregate((left, right) => left.CombineErrors(right));
            };
        }
    }
}
