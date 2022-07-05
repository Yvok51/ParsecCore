using ParsecCore.EitherNS;
using System;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    class CountParser
    {
        public static Parser<IReadOnlyList<T>> Parser<T>(Parser<T> parser, int count)
        {
            if (count <= 0)
            {
                return Parsers.Return<IReadOnlyList<T>>(Array.Empty<T>());
            }
            return (input) =>
            {
                T[] result = new T[count];

                int i = 0;
                do
                {
                    var parseResult = parser(input);
                    if (parseResult.HasLeft)
                    {
                        return Either.Error<ParseError, IReadOnlyList<T>>(parseResult.Left);
                    }
                    result[i] = parseResult.Right;
                    i++;
                } while (i < count);

                return Either.Result<ParseError, IReadOnlyList<T>>(result);
            };
        }
    }
}
