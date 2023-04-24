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

                List<IResult<Maybe<(Func<T, T, T> op, T value)>, TInputToken>> parsedResults = new();
                var rightSideResult = opParser(leftValue.UnconsumedInput);
                parsedResults.Add(rightSideResult);
                while (rightSideResult.IsResult && rightSideResult.Result.HasValue)
                {
                    rightSideResult = opParser(rightSideResult.UnconsumedInput);
                    parsedResults.Add(rightSideResult);
                }

                if (rightSideResult.IsError)
                {
                    return Result.Failure<T, Maybe<(Func<T, T, T>, T)>, TInputToken>(parsedResults);
                }

                if (parsedResults.Count == 1)
                {
                    return Result.Success(leftValue.Result, leftValue, rightSideResult);
                }
                else if (parsedResults.Count == 2)
                {
                    return Result.Success(
                        parsedResults[0].Result.Value.op(leftValue.Result, parsedResults[0].Result.Value.value),
                        parsedResults,
                        leftValue,
                        rightSideResult.UnconsumedInput
                    );
                }

                T accum = parsedResults[parsedResults.Count - 2].Result.Value.op(
                    parsedResults[parsedResults.Count - 3].Result.Value.value,
                    parsedResults[parsedResults.Count - 2].Result.Value.value
                );
                for (int i = parsedResults.Count - 3; i >= 1 ; i--)
                {
                    accum = parsedResults[i].Result.Value.op(parsedResults[i - 1].Result.Value.value, accum);
                }

                return Result.Success(
                    parsedResults[0].Result.Value.op(leftValue.Result, accum),
                    parsedResults,
                    leftValue,
                    rightSideResult.UnconsumedInput
                );
            };
        }
    }
}
