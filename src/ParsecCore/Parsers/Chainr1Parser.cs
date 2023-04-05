﻿using System;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parses one or more occurences of the given values seperated by operators
    /// Returns a value obtained by <em>right-associative</em>
    /// application of the functions returned by function parser.
    /// </summary>
    internal class Chainr1Parser
    {
        private static Parser<T, TInputToken> parseNextValue<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op
        )
        {
            return from x in value
                   from y in parseNextOp(x, value, op)
                   select y;
        }
        private static Parser<T, TInputToken> parseNextOp<T, TInputToken>(
            T left,
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op
        )
        {
            var nexOpParser = from f in op
                              from right in parseNextValue(value, op)
                              select f(left, right);
            return Parsers.Choice(nexOpParser, Parsers.Return<T, TInputToken>(left));
        }
        public static Parser<T, TInputToken> Parser<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op
        )
        {
            return parseNextValue(value, op);
        }
    }
}
