using ParsecCore.EitherNS;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser tries to parse all of the given parsers in a sequence
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class AllParser
    {
        public static Parser<IReadOnlyList<T>> Parser<T>(params Parser<T>[] parsers)
        {
            return Parser((IEnumerable<Parser<T>>)parsers);
        }

        public static Parser<IReadOnlyList<T>> Parser<T>(IEnumerable<Parser<T>> parsers)
        {
            return (input) =>
            {
                List<T> result = new List<T>();

                foreach (var parser in parsers)
                {
                    var parsedResult = parser(input);
                    if (parsedResult.IsError)
                    {
                        return Either.Error<ParseError, IReadOnlyList<T>>(parsedResult.Error);
                    }

                    result.Add(parsedResult.Result);
                }

                return Either.Result<ParseError, IReadOnlyList<T>>(result);
            };
        }
    }
}
