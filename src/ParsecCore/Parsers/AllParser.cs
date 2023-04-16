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

        public static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TInputToken>(
            IReadOnlyList<Parser<T, TInputToken>> parsers
        )
        {
            return (input) =>
            {
                T[] result = new T[parsers.Count];

                for (int i = 0; i < result.Length; i++)
                {
                    var parsedResult = parsers[i](input);
                    input = parsedResult.UnconsumedInput;
                    if (parsedResult.IsError)
                    {
                        return Result.RetypeError<T, IReadOnlyList<T>, TInputToken>(parsedResult);
                    }

                    result[i] = parsedResult.Result;
                }

                return Result.Success(result, input);
            };
        }
    }
}
