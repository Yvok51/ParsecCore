using System;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parses one or more occurences of the given values seperated by operators
    /// Returns a value obtained by <em>right-associative</em>
    /// application of the functions returned by function parser.
    /// </summary>
    internal class Chainr1Parser
    {
        public static Parser<T, TInputToken> Parser<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op
        )
        {
            Parser<Maybe<(Func<T, T, T> op, T value)>, TInputToken> opParser =
                (from f in op
                from right in value
                select (f, right)).Optional();

            return (input) =>
            {
                var leftValue = value(input);
                if (leftValue.IsError)
                {
                    return leftValue;
                }

                List<(Func<T, T, T> op, T value)> parsedResults = new();
                var rightSideResult = opParser(leftValue.UnconsumedInput);
                while (rightSideResult.IsResult && rightSideResult.Result.HasValue)
                {
                    parsedResults.Add(rightSideResult.Result.Value);
                    rightSideResult = opParser(rightSideResult.UnconsumedInput);
                }

                if (rightSideResult.IsError)
                {
                    return Result.RetypeError<Maybe<(Func<T, T, T>, T)>, T, TInputToken>(rightSideResult);
                }

                if (parsedResults.Count == 0)
                {
                    return Result.Success(leftValue.Result, rightSideResult.UnconsumedInput);
                }
                else if (parsedResults.Count == 1)
                {
                    return Result.Success(
                        parsedResults[0].op(leftValue.Result, parsedResults[0].value),
                        rightSideResult.UnconsumedInput
                    );
                }

                T accum = parsedResults[parsedResults.Count - 1].op(
                    parsedResults[parsedResults.Count - 2].value,
                    parsedResults[parsedResults.Count - 1].value
                );
                for (int i = parsedResults.Count - 2; i >= 1 ; i--)
                {
                    accum = parsedResults[i].op(parsedResults[i - 1].value, accum);
                }

                return Result.Success(parsedResults[0].op(leftValue.Result, accum), rightSideResult.UnconsumedInput);
            };
        }
    }
}
