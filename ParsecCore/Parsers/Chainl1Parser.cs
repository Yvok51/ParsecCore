using System;

namespace ParsecCore.ParsersHelp
{
    class Chainl1Parser
    {
        private static Parser<T> parseNextOpValue<T>(
            T left,
            Parser<T> value,
            Parser<Func<T, T, T>> op
        )
        {
            var nexOpParser = from f in op
                              from right in value
                              from result in parseNextOpValue(f(left, right), value, op)
                              select result;
            return Combinators.Choice(nexOpParser, Parsers.Return(left));
        }
        public static Parser<T> Parser<T>(
            Parser<T> value,
            Parser<Func<T, T, T>> op
        )
        {
            return from left in value
                   from result in parseNextOpValue(left, value, op)
                   select result;
        }
    }
}
