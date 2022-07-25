using ParsecCore.EitherNS;
using System;

namespace ParsecCore.ParsersHelp
{
    class BindParser
    {
        public static Parser<TResult, TInputToken> Parser<TFirst, TSecond, TResult, TInputToken>(
            Parser<TFirst, TInputToken> first,
            Func<TFirst, Parser<TSecond, TInputToken>> getSecond,
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
