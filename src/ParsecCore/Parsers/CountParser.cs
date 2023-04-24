using ParsecCore.Help;
using System;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which tries to apply the given parser a certain number of times.
    /// If the application the parser fails before it was applied the given number of times,
    /// then the entire parser fails.
    /// If count is less then 0, then returns <see cref="Parsers.Return{T, TInputToken}(T)"/> parser
    /// that only returns an empty list.
    /// </summary>
    internal class CountParser
    {
        public static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TInputToken>(
            Parser<T, TInputToken> parser, int count
        )
        {
            if (count <= 0)
            {
                return Parsers.Return<IReadOnlyList<T>, TInputToken>(Array.Empty<T>());
            }
            return (input) =>
            {
                List<IResult<T, TInputToken>> results = new(count);

                var parseResult = parser(input);
                if (parseResult.IsError)
                {
                    return Result.RetypeError<T, IReadOnlyList<T>, TInputToken>(parseResult);
                }
                results.Add(parseResult);
                for (int i = 1; i < count; i++)
                {
                    parseResult = parser(parseResult.UnconsumedInput);
                    results.Add(parseResult);
                    if (parseResult.IsError)
                    {
                        return Result.Failure<IReadOnlyList<T>, T, TInputToken>(results);
                    }
                }
                return Result.Success(results.Map(res => res.Result), results, parseResult.UnconsumedInput);
            };
        }
    }
}
