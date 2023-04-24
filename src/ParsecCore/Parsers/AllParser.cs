using ParsecCore.Help;
using System;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser tries to parse all of the given parsers in a sequence
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class AllParser
    {
        public static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TInputToken>(
            params Parser<T, TInputToken>[] parsers
        )
        {
            return Parser((IReadOnlyList<Parser<T, TInputToken>>)parsers);
        }

        public static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TInputToken>(
            IEnumerable<Parser<T, TInputToken>> parsers
        )
        {
            return Parser(parsers, 8);
        }

        public static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TInputToken>(
            IReadOnlyList<Parser<T, TInputToken>> parsers
        )
        {
            return Parser(parsers, parsers.Count);
        }

        private static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TInputToken>(
            IEnumerable<Parser<T, TInputToken>> parsers,
            int capacity
        )
        {
            Func<IResult<T, TInputToken>, T> map = res => res.Result;
            return (input) =>
            {
                List<IResult<T, TInputToken>> results = new(capacity);

                foreach (var parser in parsers)
                {
                    var parsedResult = parser(input);
                    input = parsedResult.UnconsumedInput;
                    results.Add(parsedResult);
                    if (parsedResult.IsError)
                    {
                        return Result.Failure<IReadOnlyList<T>, T, TInputToken>(results);
                    }
                }

                return Result.Success(results.Map(map), results, input);
            };
        }
    }
}
