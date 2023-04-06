using ParsecCore.EitherNS;
using ParsecCore.Input;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Returns a parser which tries to first apply the first parser and if it succeeds returns the result.
    /// If it fails <strong>and does not consume any input</strong> then the second parser is applied and so on.
    /// If any parser fails while consuming input, then the parser's error is returned.
    /// If all parsers fail then combines the errors of all the parsers
    /// </summary>
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
                List<IResult<T, TInputToken>> parseResults = new();
                foreach (var parser in parsers)
                {
                    IResult<T, TInputToken> currentParseResult = parser(input);
                    parseResults.Add(currentParseResult);
                    if (currentParseResult.IsResult)
                    {
                        return currentParseResult;
                    }
                    if (!input.Equals(currentParseResult.UnconsumedInput))
                    {
                        return parseResults.Aggregate((left, right) => left.CombineErrors(right));
                    }

                }
                if (parseResults.Count == 0)
                {
                    return Result.Failure<T, TInputToken>(new CustomError(input.Position, new FailWithError("No parser provided")), input);
                }

                return parseResults.Aggregate((left, right) => left.CombineErrors(right));
            };
        }
    }
}
