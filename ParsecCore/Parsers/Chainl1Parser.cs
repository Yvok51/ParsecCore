using System;

namespace ParsecCore.ParsersHelp
{
    class Chainl1Parser
    {
        private static Parser<T, TInputToken> parseNextOpValue<T, TInputToken>(
            T left,
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op
        )
        {
            var nexOpParser = from f in op
                              from right in value
                              from result in parseNextOpValue(f(left, right), value, op)
                              select result;
            return Combinators.Choice(nexOpParser, Parsers.Return<T, TInputToken>(left));
        }
        public static Parser<T, TInputToken> Parser<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op
        )
        {
            return from left in value
                   from result in parseNextOpValue(left, value, op)
                   select result;
        }
    }
}
