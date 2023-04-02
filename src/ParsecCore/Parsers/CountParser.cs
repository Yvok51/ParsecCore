using ParsecCore.EitherNS;
using System;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
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
                        return Either.Error<ParseError, IReadOnlyList<T>>(parseResult.Error);
                    }
                    result[i] = parseResult.Result;
                    i++;
                } while (i < count);

                return Either.Result<ParseError, IReadOnlyList<T>>(result);
            };
        }
    }
}
