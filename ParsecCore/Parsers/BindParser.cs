using ParsecCore.EitherNS;
using System;

namespace ParsecCore.ParsersHelp
{
    class BindParser
    {
        public static Parser<TResult> Parser<TFirst, TSecond, TResult>(
            Parser<TFirst> first,
            Func<TFirst, Parser<TSecond>> getSecond,
            Func<TFirst, TSecond, TResult> getResult
        )
        {
            return (input) =>
            {
                return from firstValue in first(input)
                       from secondValue in getSecond(firstValue)(input)
                       select getResult(firstValue, secondValue);
            };
        }
    }
}
