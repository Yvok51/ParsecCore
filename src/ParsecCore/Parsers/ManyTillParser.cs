using ParsecCore.Help;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// See <see cref="Parsers.ManyTill{T, TEnd, TInputToken}(Parser{T, TInputToken}, Parser{TEnd, TInputToken})"/>
    /// for details.
    /// </summary>
    internal class ManyTillParser
    {
        public static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TEnd, TInputToken>(
            Parser<T, TInputToken> many,
            Parser<TEnd, TInputToken> till
        )
        {
            Parser<TEnd, TInputToken> tryTill = till.Try();
            return (input) =>
            {
                List<IResult<T, TInputToken>> results = new();

                var tillResult = tryTill(input);
                while (tillResult.IsError)
                {
                    // tryTill cannot consume input -> no need to check if it does
                    var manyResult = many(tillResult.UnconsumedInput);
                    results.Add(manyResult);
                    if (manyResult.IsError)
                    {
                        return Result.Failure<IReadOnlyList<T>, T, TInputToken>(results);
                    }

                    tillResult = tryTill(manyResult.UnconsumedInput);
                }

                return Result.Success(results.Map(res => res.Result), results, tillResult, tillResult.UnconsumedInput);
            };
        }
    }
}
