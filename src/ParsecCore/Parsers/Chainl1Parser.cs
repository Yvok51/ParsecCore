using System;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parses one or more occurences of the given values seperated by operators
    /// Returns a value obtained by <em>left-associative</em>
    /// application of the functions returned by function parser.
    /// Especially useful for parsing left-recursive grammars, which are often used in numerical expressions.
    /// </summary>
    internal class Chainl1Parser
    {
        public static Parser<T, TInputToken> Parser<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op
        )
        {
            Parser<Maybe<(Func<T, T, T> op, T right)>, TInputToken> opParser = 
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

                List<IResult<Maybe<(Func<T, T, T> op, T right)>, TInputToken>> results = new();

                T accum = leftValue.Result;
                var rightSideResult = opParser(leftValue.UnconsumedInput);
                results.Add(rightSideResult);
                while (rightSideResult.IsResult && rightSideResult.Result.HasValue)
                {
                    accum = rightSideResult.Result.Value.op(accum, rightSideResult.Result.Value.right);
                    rightSideResult = opParser(rightSideResult.UnconsumedInput);
                    results.Add(rightSideResult);
                }

                if (rightSideResult.IsError)
                {
                    return Result.Failure<T, Maybe<(Func<T, T, T>, T)>, TInputToken>(results);
                }

                return Result.Success(accum, results, leftValue, rightSideResult.UnconsumedInput);
            };
        }
    }
}
