using System.Collections.Generic;

using ParsecCore.EitherNS;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser tries to parse all of the given parsers in a sequence
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class AllParser
    {
        public static Parser<IEnumerable<T>> Parser<T>(params Parser<T>[] parsers)
        {
            return Parser((IEnumerable<Parser<T>>)parsers);
        }

        public static Parser<IEnumerable<T>> Parser<T>(IEnumerable<Parser<T>> parsers)
        {
            return (input) =>
            {
                List<T> result = new List<T>();

                foreach (var parser in parsers)
                {
                    var parsedResult = parser(input);
                    if (parsedResult.HasLeft)
                    {
                        return Either.Error<ParseError, IEnumerable<T>>(parsedResult.Left);
                    }

                    result.Add(parsedResult.Right);
                }

                return Either.Result<ParseError, IEnumerable<T>>(result);
            };
        }
    }
}
