using ParsecCore.ParsersHelp;
using System;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Parses zero or more occurences of the given values seperated by operators
        /// Returns a value obtained by <em>left-associative</em>
        /// application of the functions returned by <paramref name="op"/>.
        /// If there are zero occurences, then the <paramref name="defaultValue"/> is returned
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <param name="value"> The parser for values </param>
        /// <param name="op"> Parser for the binary operators </param>
        /// <param name="defaultValue"> The default value to return in case no value is parsed </param>
        /// <returns> 
        /// Parser which returns a value obtained by the left-associative application of the functions 
        /// given by <paramref name="op"/> on values returned by <paramref name="value"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<T, TInputToken> ChainL<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op,
            T defaultValue
        ) =>
            Choice(ChainL1(value, op), Return<T, TInputToken>(defaultValue));

        /// <summary>
        /// Parses one or more occurences of the given values seperated by operators
        /// Returns a value obtained by <em>left-associative</em>
        /// application of the functions returned by <paramref name="op"/>.
        /// Especially useful for parsing left-recursive grammars, which are often used in numerical expressions.
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <param name="value"> The parser for values </param>
        /// <param name="op"> Parser for the binary operators </param>
        /// <returns> 
        /// Parser which returns a value obtained by the left-associative application of the functions 
        /// given by <paramref name="op"/> on values returned by <paramref name="value"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<T, TInputToken> ChainL1<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op
        )
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (op is null) throw new ArgumentNullException(nameof(op));

            return Chainl1Parser.Parser(value, op);
        }

        /// <summary>
        /// Parses zero or more occurences of the given values seperated by operators
        /// Returns a value obtained by <em>right-associative</em>
        /// application of the functions returned by <paramref name="op"/>.
        /// If there are zero occurences, then the <paramref name="defaultValue"/> is returned
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <param name="value"> The parser for values </param>
        /// <param name="op"> Parser for the binary operators </param>
        /// <param name="defaultValue"> The default value to return in case no value is parsed </param>
        /// <returns> 
        /// Parser which returns a value obtained by the right-associative application of the functions 
        /// given by <paramref name="op"/> on values returned by <paramref name="value"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<T, TInputToken> ChainR<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op,
            T defaultValue
        ) =>
            Choice(ChainR1(value, op), Return<T, TInputToken>(defaultValue));

        /// <summary>
        /// Parses one or more occurences of the given values seperated by operators
        /// Returns a value obtained by <em>right-associative</em>
        /// application of the functions returned by <paramref name="op"/>.
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <param name="value"> The parser for values </param>
        /// <param name="op"> Parser for the binary operators </param>
        /// <returns> 
        /// Parser which returns a value obtained by the right-associative application of the functions 
        /// given by <paramref name="op"/> on values returned by <paramref name="value"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<T, TInputToken> ChainR1<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op
        )
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (op is null) throw new ArgumentNullException(nameof(op));

            return Chainr1Parser.Parser(value, op);
        }
    }
}
