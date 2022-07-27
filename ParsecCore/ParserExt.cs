﻿using ParsecCore.EitherNS;
using ParsecCore.Help;
using ParsecCore.MaybeNS;
using ParsecCore.ParsersHelp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore
{
    public static class ParserExt
    {
        /// <summary>
        /// Changes the error message returned by the parser if it fails.
        /// The parser modified by this method behaves the same except that when it fails,
        /// the new expected message is used 
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <typeparam name="TInputToken"> The type of the token returned by the parser's input </typeparam>
        /// <param name="parser"> The parser whose error message to change </param>
        /// <param name="newMessage"> The new error message </param>
        /// <returns> Parser which upon failure returns ParseError with modified error message </returns>
        public static Parser<T, TInputToken> FailWith<T, TInputToken>(
            this Parser<T, TInputToken> parser,
            ErrorMessage newMessage
        )
        {
            return (input) =>
            {
                var result = parser(input);
                if (result.IsResult)
                {
                    return result;
                }

                return Either.Error<ParseError, T>(result.Error.WithErrorMessage(newMessage));
            };
        }

        /// <summary>
        /// Extension method enabling us to use the LINQ syntax for the parsers.
        /// The LINQ syntax imitates 'do' notation found in haskell.
        /// <c>for x in xs</c> is equivalent to <c>x <- xs</c> and <c>select x</c> is equivalent to <c>return x</c>
        /// This method is therefore similar to <c>bind</c> <c>(>>=)</c>
        /// </summary>
        /// <typeparam name="TSource"> > The type of the source parser </typeparam>
        /// <typeparam name="TResult"> The type of the resulting parser </typeparam>
        /// <param name="parser"> The source parser </param>
        /// <param name="projection"> The funtion to map the result of the source parser with </param>
        /// <returns> Parser which maps the result of the source parser to a new value </returns>
        public static Parser<TResult, TInputToken> Select<TSource, TResult, TInputToken>(
            this Parser<TSource, TInputToken> parser,
            Func<TSource, TResult> projection
        ) =>
            MapParser.Parser(parser, projection);

        /// <summary>
        /// Extension method enabling us to use the LINQ syntax for the parsers.
        /// The LINQ syntax imitates 'do' notation found in haskell.
        /// <c>for x in xs</c> is equivalent to <c>x <- xs</c> and <c>select x</c> is equivalent to <c>return x</c>
        /// This method is therefore similar to <c>bind</c> <c>(>>=)</c>
        /// </summary>
        /// <typeparam name="TFirst"> The type of the source parser </typeparam>
        /// <typeparam name="TSecond"> The type of the parser returned by the chained method </typeparam>
        /// <typeparam name="TResult"> The type of the resulting parser </typeparam>
        /// <param name="first"> The source parser </param>
        /// <param name="getSecond"> The function to chain to the result of the source parser </param>
        /// <param name="getResult"> Callback which combines the two results together </param>
        /// <returns> Parser which performs the source parser and afterwards the chained method in that order </returns>
        public static Parser<TResult, TInputToken> SelectMany<TFirst, TSecond, TResult, TInputToken>(
            this Parser<TFirst, TInputToken> first,
            Func<TFirst, Parser<TSecond, TInputToken>> getSecond,
            Func<TFirst, TSecond, TResult> getResult
        ) =>
            BindParser.Parser(first, getSecond, getResult);

        /// <summary>
        /// Returns a parser which tries to parse according to the given parser as many times as possible.
        /// Can parse even zero times.
        /// If parsing fails in the middle of one iteration (some input was consumed),
        /// then the parser fails regardless of how many previous iterations were parsed already.
        /// If this behaviour is not desired add the modifier <see cref="Try{T}(Parser{T})"/> 
        /// to the parser beforehand.
        /// </summary>
        /// <typeparam name="T"> The type of the parser </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <returns> Parser which applies the given parser as many times as possible </returns>
        public static Parser<IReadOnlyList<T>, TInputToken> Many<T, TInputToken>(this Parser<T, TInputToken> parser) =>
            ManyParser.Parser(parser);

        /// <summary>
        /// Returns a parser which tries to parse according to the given parser as many times as possible.
        /// Must be sucessful at least once otherwise an error is returned.
        /// If parsing fails in the middle of one iteration (some input was consumed),
        /// then the parser fails regardless of how many previous iterations were parsed already.
        /// If this behaviour is not desired add the modifier <see cref="Try{T}(Parser{T})"/> 
        /// to the parser beforehand.
        /// </summary>
        /// <typeparam name="T"> The type of the parser </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <returns> Parser which applies the given parser as many times as possible </returns>
        public static Parser<IReadOnlyList<T>, TInputToken> Many1<T, TInputToken>(this Parser<T, TInputToken> parser)
        {
            return from firstParse in parser
                   from restParses in parser.Many()
                   select restParses.Prepend(firstParse);
        }

        /// <summary>
        /// Specialization of the <see cref="Many{T}(Parser{T})">Many</see> method for chars and strings
        /// </summary>
        /// <param name="parser"> The parser to apply as many times as possible </param>
        /// <returns> Parser which applies the given char parser as many times as possible </returns>
        public static Parser<string, TInputToken> Many<TInputToken>(this Parser<char, TInputToken> parser)
        {
            return from chars in parser.Many<char, TInputToken>()  // added explicit char to avoid recursion
                   select string.Concat(chars);
        }

        /// <summary>
        /// Specialization of the <see cref="Many1{T}(Parser{T})">Many1</see> method for chars and strings
        /// </summary>
        /// <param name="parser"> The parser to apply as many times as possible </param>
        /// <returns> Parser which applies the given char parser as many times as possible </returns>
        public static Parser<string, TInputToken> Many1<TInputToken>(this Parser<char, TInputToken> parser)
        {
            return from firstParse in parser
                   from restParses in parser.Many()
                   select firstParse.ToString() + restParses;
        }

        /// <summary>
        /// Aplies the parser as many times as possible and ignores the result.
        /// <see cref="Many{T}(Parser{T})"/> for specifics.
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <returns> 
        /// Parser which applies the given parser as many times as possible and then ignores its result.
        /// </returns>
        public static Parser<None, TInputToken> SkipMany<T, TInputToken>(this Parser<T, TInputToken> parser)
        {
            return from _ in parser.Many()
                   select new None();
        }

        /// <summary>
        /// Applies the parser as many times as possible (but at least once) and ignores the result.
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <returns> 
        /// Parser which applies the given parser as many times as possible and then ignores its result.
        /// </returns>
        public static Parser<None, TInputToken> SkipMany1<T, TInputToken>(this Parser<T, TInputToken> parser)
        {
            return from _ in parser.Many1()
                   select new None();
        }

        /// <summary>
        /// Aplies the parser a given amount of times.
        /// If the count is zero or less, then the returned parser is equivalent to
        /// the <see cref="Parsers.Return{T}(T)">ReturnParser</see> of an empty enumerable
        /// </summary>
        /// <typeparam name="T"> The type of parser to apply </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <param name="count"> Number of times to apply the parser </param>
        /// <returns></returns>
        public static Parser<IReadOnlyList<T>, TInputToken> Count<T, TInputToken>(this Parser<T, TInputToken> parser, int count) =>
            CountParser.Parser(parser, count);

        /// <summary>
        /// Returns a parser which either parses its value or returns 
        /// Nothing if the underlying parser does not consume any input or propagates
        /// the error if any input was consumed
        /// </summary>
        /// <typeparam name="T"> The result type of the parser </typeparam>
        /// <param name="parser"> The parser to optionally apply </param>
        /// <returns> Parser which optionally applies the given parser </returns>
        public static Parser<IMaybe<T>, TInputToken> Optional<T, TInputToken>(this Parser<T, TInputToken> parser) =>
            OptionalParser.Parser(parser);

        /// <summary>
        /// Tries to parse according to the given parser. If the parser succeeds, then returns the parser's result.
        /// If the parser fails <strong>without consuming input</strong> then returns the given default value.
        /// If the parser fails while consuming input, the failure is propagated upwards.
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <param name="parser"> The parser to modify </param>
        /// <param name="defaultValue"> The default value to return if the parser fails </param>
        /// <returns> Parser which returns a default value upon failure </returns>
        public static Parser<T, TInputToken> Option<T, TInputToken>(this Parser<T, TInputToken> parser, T defaultValue)
        {
            return from opt in parser.Optional()
                   select opt.Else(defaultValue);
        }

        /// <summary>
        /// Makes the parser not consume any input if it fails.
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <param name="parser"> The parser to modify </param>
        /// <returns> Parser which does not consume any input on failure </returns>
        public static Parser<T, TInputToken> Try<T, TInputToken>(this Parser<T, TInputToken> parser) =>
            (input) =>
            {
                var initialPosition = input.Position;
                var result = parser(input);
                if (result.IsResult)
                {
                    return result;
                }

                if (input.Position != initialPosition)
                {
                    input.Seek(initialPosition);
                }
                return result;
            };

        /// <summary>
        /// Parses <c>parser</c> without consuming input.
        /// If <c>parser</c> fails and consumes input, then so does lookAhead
        /// (combine with <see cref="Try{T}(Parser{T})"/> if this is undesirable)
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <param name="parser"> Parser to look ahead with </param>
        /// <returns> Parser which looks ahead (parses without consuming input) </returns>
        public static Parser<T, TInputToken> LookAhead<T, TInputToken>(this Parser<T, TInputToken> parser)
        {
            return (input) =>
            {
                var initialPosition = input.Position;
                var result = parser(input);
                if (result.IsResult)
                {
                    input.Seek(initialPosition);
                }

                return result;
            };
        }

        public static Parser<T, TInputToken> Or<T, TInputToken>(
            this Parser<T, TInputToken> parser,
            Parser<T, TInputToken> secondParser
        )
        {
            return (input) =>
            {
                var initialPosition = input.Position;
                var firstResult = parser(input);
                if (firstResult.IsResult || (firstResult.IsError && initialPosition != input.Position))
                {
                    return firstResult;
                }

                var secondResult = secondParser(input);
                if (secondResult.IsError)
                {
                    return firstResult.CombineErrors(secondResult);
                }

                return secondResult;
            };
        }
    }
}
