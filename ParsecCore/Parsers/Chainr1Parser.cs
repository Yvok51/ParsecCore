using System;

namespace ParsecCore.ParsersHelp
{
    class Chainr1Parser
    {
        private static Parser<T> parseNextValue<T>(
            Parser<T> value,
            Parser<Func<T, T, T>> op
        )
        {
            return from x in value
                   from y in parseNextOp(x, value, op)
                   select y;
        }
        private static Parser<T> parseNextOp<T>(
            T left,
            Parser<T> value,
            Parser<Func<T, T, T>> op
        )
        {
            var nexOpParser = from f in op
                              from right in parseNextValue(value, op)
                              select f(left, right);
            return Combinators.Choice(nexOpParser, Parsers.Return(left));
        }
        public static Parser<T> Parser<T>(
            Parser<T> value,
            Parser<Func<T, T, T>> op
        )
        {
            return parseNextValue(value, op);
        }
    }
}
