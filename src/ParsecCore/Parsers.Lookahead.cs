﻿using System;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Makes the parser not consume any input if it fails.
        /// If the parser succeeds, then consumes input as usual.
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to modify </param>
        /// <returns> Parser which does not consume any input on failure </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<T, TInputToken> Try<T, TInputToken>(
            this Parser<T, TInputToken> parser
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return (input) =>
            {
                var result = parser(input);
                if (result.IsResult)
                {
                    return result;
                }

                return Result.Failure<T, TInputToken>(result.Error!, input);
            };
        }

        /// <summary>
        /// Parses <paramref name="parser"/> without consuming input.
        /// Returns the parsed result but the input remains the same and has to be parsed again.
        /// If <paramref name="parser"/> fails and consumes input, then so does lookAhead
        /// (combine with <see cref="Try{T, TInputToken}(Parser{T, TInputToken})"/> if this is undesirable)
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> Parser to look ahead with </param>
        /// <returns> Parser which looks ahead (parses without consuming input) </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<T, TInputToken> LookAhead<T, TInputToken>(
            this Parser<T, TInputToken> parser
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return (input) =>
            {
                var result = parser(input);
                if (result.IsResult)
                {
                    Result.Success(result.Result, result.Error, input);
                }

                return result;
            };
        }

        /// <summary>
        /// This parser fails if <paramref name="parser"/> succeeds. It does not consume any input.
        /// </summary>
        /// <typeparam name="T"> The return type of parser </typeparam>
        /// <param name="parser"> Parser which should not succeed </param>
        /// <param name="msgIfParsed"> The error message to use if <c>parser</c> succeeds </param>
        /// <returns> Parser which fails if <c>parser</c> succeeds </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<None, TInputToken> NotFollowedBy<T, TInputToken>(
            Parser<T, TInputToken> parser,
            string msgIfParsed
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (msgIfParsed is null) throw new ArgumentNullException(nameof(msgIfParsed));

            var failParser = parser.Try().Then(Fail<None, TInputToken>(msgIfParsed));

            return Choice(failParser, Return<None, TInputToken>(None.Instance)).Try();
        }
    }
}
