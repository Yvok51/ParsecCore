using ParsecCore.Help;
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
                List<IResult<Maybe<T>, TInputToken>> results = new();

                var parseResult = optParser(input);
                results.Add(parseResult);
                while (parseResult.IsResult && !parseResult.Result.IsEmpty)
                {
                    parseResult = optParser(parseResult.UnconsumedInput);
                    results.Add(parseResult);
                }

                if (parseResult.IsError)
                {
                    return Result.Failure<IReadOnlyList<T>, Maybe<T>, TInputToken>(results);
                }

                results.RemoveAt(results.Count - 1);
                return Result.Success(
                    results.Map(res => res.Result.Value),
                    results,
                    parseResult,
                    parseResult.UnconsumedInput
                );
            };
        }
    }
}
