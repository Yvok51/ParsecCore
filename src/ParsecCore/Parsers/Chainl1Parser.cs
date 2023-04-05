using System;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parses one or more occurences of the given values seperated by operators
    /// Returns a value obtained by <em>left-associative</em>
    /// application of the functions returned by function parser.
    /// Especially useful for parsing left-recursive grammars, which are often used in numerical expressions.
    /// </summary>
    internal class Chainl1Parser
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
            return Parsers.Choice(nexOpParser, Parsers.Return<T, TInputToken>(left));
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
