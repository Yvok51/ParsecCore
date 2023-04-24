using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which takes a parser and attempts to apply it as many times as possible 
    /// Used in the implementation of the Many method.
    /// </summary>
    internal class ManyParser
    {
        public static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TInputToken>(Parser<T, TInputToken> parser)
        {
            Parser<Maybe<T>, TInputToken> optParser = parser.Optional();
            return (input) =>
            {
                List<IResult<Maybe<T>, TInputToken>> parseResults = new();
                List<T> results = new();

                var parseResult = optParser(input);
                parseResults.Add(parseResult);
                while (parseResult.IsResult && !parseResult.Result.IsEmpty)
                {
                    results.Add(parseResult.Result.Value);
                    parseResult = optParser(parseResult.UnconsumedInput);
                    parseResults.Add(parseResult);
                }

                if (parseResult.IsError)
                {
                    return Result.Failure<IReadOnlyList<T>, Maybe<T>, TInputToken>(parseResults);
                }

                return Result.Success(
                    results,
                    parseResults
                );
            };
        }
    }
}
