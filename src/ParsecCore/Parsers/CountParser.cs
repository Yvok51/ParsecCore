using ParsecCore.EitherNS;
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
                T[] result = new T[count];

                int i = 0;
                do
                {
                    var parseResult = parser(input);
                    if (parseResult.IsError)
                    {
                        return Either.RetypeError<ParseError, T, IReadOnlyList<T>>(parseResult);
                    }
                    result[i] = parseResult.Result;
                    i++;
                } while (i < count);

                return Either.Result<ParseError, IReadOnlyList<T>>(result);
            };
        }
    }
}
