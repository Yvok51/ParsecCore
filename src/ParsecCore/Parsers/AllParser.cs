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
            return Parser((IEnumerable<Parser<T, TInputToken>>)parsers);
        }

        public static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TInputToken>(
            IEnumerable<Parser<T, TInputToken>> parsers
        )
        {
            return (input) =>
            {
                List<T> result = new List<T>();

                foreach (var parser in parsers)
                {
                    var parsedResult = parser(input);
                    input = parsedResult.UnconsumedInput;
                    if (parsedResult.IsError)
                    {
                        return Result.RetypeError<T, IReadOnlyList<T>, TInputToken>(parsedResult);
                    }

                    result.Add(parsedResult.Result);
                }

                return Result.Success(result, input);
            };
        }
    }
}
