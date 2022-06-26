using System;
using System.Collections.Generic;

using ParsecCore.EitherNS;

namespace ParsecCore.ParsersHelp
{
    class CountParser
    {
        public static Parser<IEnumerable<T>> Parser<T>(Parser<T> parser, int count)
        {
            if (count <= 0)
            {
                return Parsers.Return<IEnumerable<T>>(Array.Empty<T>());
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
                        return Either.Error<ParseError, IEnumerable<T>>(parseResult.Left);
                    }
                    result[i] = parseResult.Right;
                    i++;
                } while (i < count);

                return Either.Result<ParseError, IEnumerable<T>>(result);
            };
        }
    }
}
