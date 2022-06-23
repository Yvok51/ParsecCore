using System;
using System.Collections.Generic;
using System.Linq;

using ParsecCore.MaybeNS;
using ParsecCore.HelpParsers;

namespace ParsecCore
{
    public static class ParserExt
    {
        /// <summary>
        /// Extension method enabling us to use the LINQ syntax for the parsers.
        /// The LINQ syntax imitates 'do' notation found in haskell.
        /// `for x in xs` is equivalent to `x <- xs` and `select x` is equivalent to `return x`
        /// This method is similar to map
        /// </summary>
        /// <typeparam name="TSource"> > The type of the source parser </typeparam>
        /// <typeparam name="TResult"> The type of the resulting parser </typeparam>
        /// <param name="parser"> The source parser </param>
        /// <param name="projection"> The funtion to map the result of the source parser with </param>
        /// <returns> Parser which maps the result of the source parser to a new value </returns>
        public static IParser<TResult> Select<TSource, TResult>(
            this IParser<TSource> parser,
            Func<TSource, TResult> projection
        )
        {
            return new MapParser<TSource, TResult>(parser, projection);
        }

        /// <summary>
        /// Extension method enabling us to use the LINQ syntax for the parsers.
        /// The LINQ syntax imitates 'do' notation found in haskell.
        /// `for x in xs` is equivalent to `x <- xs` and `select x` is equivalent to `return x`
        /// This method is therefore similar to 'bind' (>>=)
        /// </summary>
        /// <typeparam name="TFirst"> The type of the source parser </typeparam>
        /// <typeparam name="TSecond"> The type of the parser returned by the chained method </typeparam>
        /// <typeparam name="TResult"> The type of the resulting parser </typeparam>
        /// <param name="first"> The source parser </param>
        /// <param name="getSecond"> The function to chain to the result of the source parser </param>
        /// <param name="getResult"> Callback which combines the two results together </param>
        /// <returns> Parser which performs the source parser and afterwards the chained method in that order </returns>
        public static IParser<TResult> SelectMany<TFirst, TSecond, TResult>(
            this IParser<TFirst> first,
            Func<TFirst, IParser<TSecond>> getSecond,
            Func<TFirst, TSecond, TResult> getResult)
        {
            return new BindParser<TFirst, TSecond, TResult>(first, getSecond, getResult);
        }

        /// <summary>
        /// Returns a parser which tries to parse according to the given parser as many times as possible.
        /// Can parse even zero times.
        /// </summary>
        /// <typeparam name="T"> The type of the parser </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <returns> Parser which applies the given parser as many times as possible </returns>
        public static IParser<IEnumerable<T>> Many<T>(this IParser<T> parser)
        {
            return new ManyParser<T>(parser);   
        }

        /// <summary>
        /// Returns a parser which tries to parse according to the given parser as many times as possible.
        /// Must be sucessful at least once otherwise an error is returned.
        /// </summary>
        /// <typeparam name="T"> The type of the parser </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <returns> Parser which applies the given parser as many times as possible </returns>
        public static IParser<IEnumerable<T>> Many1<T>(this IParser<T> parser)
        {
            return from firstParse in parser
                   from restParses in parser.Many()
                   select new T[] { firstParse }.Concat(restParses);
        }

        /// <summary>
        /// Specialization of the Many method for chars and strings
        /// </summary>
        /// <param name="parser"> The parser to apply as many times as possible </param>
        /// <returns> Parser which applies the given char parser as many times as possible </returns>
        public static IParser<string> Many(this IParser<char> parser)
        {
            return from chars in parser.Many<char>()  // added explicit char to avoid recursion
                   select string.Concat(chars);
        }

        /// <summary>
        /// Specialization of the Many1 method for chars and strings
        /// </summary>
        /// <param name="parser"> The parser to apply as many times as possible </param>
        /// <returns> Parser which applies the given char parser as many times as possible </returns>
        public static IParser<string> Many1(this IParser<char> parser)
        {
            return from firstParse in parser
                   from restParses in parser.Many()
                   select firstParse.ToString() + restParses;
        }

        /// <summary>
        /// Returns a parser which either parses its value or returns Nothing and does not consume any input
        /// </summary>
        /// <typeparam name="T"> The result type of the parser </typeparam>
        /// <param name="parser"> The parser to optionally apply </param>
        /// <returns> Parser which optionally applies the given parser </returns>
        public static IParser<IMaybe<T>> Optional<T>(this IParser<T> parser)
        {
            return new OptionalParser<T>(parser);
        }
    }
}
