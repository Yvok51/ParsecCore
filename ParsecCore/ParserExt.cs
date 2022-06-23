using System;
using System.Collections.Generic;
using System.Linq;

using ParsecCore.Maybe;
using ParsecCore.Parsers;

namespace ParsecCore
{
    static class ParserExt
    {
        public static IParser<char> Whitespace = new SatisfyParser(char.IsWhiteSpace, "whitespace");
        public static IParser<char> Digit = new SatisfyParser(char.IsDigit, "digit");

        /// <summary>
        /// Return a parser which does not consume any input and only returns the value given
        /// </summary>
        /// <typeparam name="T"> The type of the value the parser returns </typeparam>
        /// <param name="value"> The value for the parser to return </param>
        /// <returns> Parser which returns the given value </returns>
        public static IParser<T> Return<T>(T value)
        {
            return new ReturnParser<T>(value);
        }

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
        /// Returns a parser which tries to first apply the first parser and if it succeeds returns the result.
        /// If it fails then the second parser is applied on the same input as the first and so on.
        /// If all parsers fail then returns the ParseError of tha last parser
        /// </summary>
        /// <typeparam name="T"> The type of the parsers </typeparam>
        /// <param name="parsers"> Parsers to apply </param>
        /// <returns> Parser which sequentally tries to apply the given parsers until one succeeds or all fails </returns>
        public static IParser<T> Choice<T>(params IParser<T>[] parsers)
        {
            return new ChoiceParser<T>(parsers);
        }

        /// <summary>
        /// Returns a parser which tries to first apply the first parser and if it succeeds returns the result.
        /// If it fails then the second parser is applied on the same input as the first and so on.
        /// If all parsers fail then returns the ParseError of tha last parser
        /// </summary>
        /// <typeparam name="T"> The type of the parsers </typeparam>
        /// <param name="parsers"> Parsers to apply </param>
        /// <returns> Parser which sequentally tries to apply the given parsers until one succeeds or all fails </returns>
        public static IParser<T> Choice<T>(IEnumerable<IParser<T>> parsers)
        {
            return new ChoiceParser<T>(parsers);
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
        /// Returns a parser which parser exactly the string given
        /// </summary>
        /// <param name="stringToParse"> The string for the parser to parse </param>
        /// <returns> Parser which parses exactly the given string </returns>
        public static IParser<string> String(string stringToParse)
        {
            Func<string, IEnumerable<IParser<char>>> stringToCharParsers = (string str) =>
            {
                IParser<char>[] parsers = new IParser<char>[str.Length];
                for (int i = 0; i < str.Length; ++i)
                {
                    parsers[i] = new CharParser(str[i]);
                }
                return parsers;
            };

            return from chars in new AllParser<char>(stringToCharParsers(stringToParse))
                   select string.Concat(chars);
        }

        /// <summary>
        /// Returns a parser which parses a value in between two other values
        /// Usefull for quoted strings, arrays, ...
        /// </summary>
        /// <typeparam name="TLeft"> The result type of the parser of the left value </typeparam>
        /// <typeparam name="TBetween"> The result type of the parser of the inbetween value </typeparam>
        /// <typeparam name="TRight"> The result type of the parser of the left value </typeparam>
        /// <param name="leftParser"> The parser for the value on the left </param>
        /// <param name="betweenParser"> The parser for the value inbetween </param>
        /// <param name="rightParser"> The parser for the value on the right </param>
        /// <returns> 
        /// Parser which parses the entire sequence of leftParse- betweenParser-rightParser but only
        /// returns the value parsed by the betweenParser
        /// </returns>
        public static IParser<TBetween> Between<TLeft, TBetween, TRight>(
            IParser<TLeft> leftParser,
            IParser<TBetween> betweenParser,
            IParser<TRight> rightParser
        )
        {
            return from l in leftParser
                   from between in betweenParser
                   from r in rightParser
                   select between;
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
