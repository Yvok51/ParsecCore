using ParsecCore.EitherNS;
using ParsecCore.ParsersHelp;
using System;
using System.Collections.Generic;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Sequences two parsers after each other but returns the result only of the second one.
        /// Apply the <paramref name="firstParser"/> and discard its result.
        /// Afterward, apply the <paramref name="secondParser"/> and return its result.
        /// If any of the parser fails, then the entire parser fails.
        /// </summary>
        /// <typeparam name="T"> The result type of the first parser </typeparam>
        /// <typeparam name="R"> The result type of the second parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parsers </typeparam>
        /// <param name="firstParser"> The parser that is applied first </param>
        /// <param name="secondParser"> The parser that is applied second and whose value is returned </param>
        /// <returns> Parser that sequences two parsers </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<R, TInput> Then<T, R, TInput>(
            this Parser<T, TInput> firstParser,
            Parser<R, TInput> secondParser
        )
        {
            if (firstParser is null) throw new ArgumentNullException(nameof(firstParser));
            if (secondParser is null) throw new ArgumentNullException(nameof(secondParser));

            return (input) =>
            {
                var discardedResult = firstParser(input);
                if (discardedResult.IsError)
                {
                    return Result.RetypeError<T, R, TInput>(discardedResult);
                }
                return secondParser(discardedResult.UnconsumedInput);
            };
        }

        /// <summary>
        /// Sequences two parsers after each other but returns the result only of the first one.
        /// Apply the <paramref name="firstParser"/>.
        /// Afterward, apply the <paramref name="secondParser"/> and discard its result.
        /// If both parsers succeeded return the result of the first one.
        /// If any of the parser fails, then the entire parser fails.
        /// </summary>
        /// <typeparam name="T"> The result type of the first parser </typeparam>
        /// <typeparam name="R"> The result type of the second parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parsers </typeparam>
        /// <param name="firstParser"> The parser that is applied first and whose value is returned </param>
        /// <param name="secondParser"> The parser that is applied second </param>
        /// <returns> Parser that sequences two parsers </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<T, TInput> FollowedBy<T, R, TInput>(
            this Parser<T, TInput> firstParser,
            Parser<R, TInput> secondParser
        )
        {
            if (firstParser is null) throw new ArgumentNullException(nameof(firstParser));
            if (secondParser is null) throw new ArgumentNullException(nameof(secondParser));

            return (input) =>
            {
                var result = firstParser(input);
                if (result.IsError)
                {
                    return result;
                }
                var discardedResult = secondParser(result.UnconsumedInput);
                if (discardedResult.IsError)
                {
                    return Result.RetypeError<R, T, TInput>(discardedResult);
                }

                return Result.Success(result.Result, discardedResult.UnconsumedInput);
            };
        }

        /// <summary>
        /// Returns a parser which tries to parse all of the <paramref name="parsers"/> in a sequence.
        /// If it succeeds it returns a list of the parsed results.
        /// If it fails it returns the first encountered parse error
        /// </summary>
        /// <typeparam name="T"> The type of parsers </typeparam>
        /// <param name="parsers"> Parsers to sequentially apply </param>
        /// <returns> Parser which sequentially aplies all of the given parsers </returns>
        /// <exception cref="ArgumentNullException"> If parser array is null </exception>
        public static Parser<IReadOnlyList<T>, TInputToken> All<T, TInputToken>(
            params Parser<T, TInputToken>[] parsers
        )
        {
            if (parsers is null) throw new ArgumentNullException(nameof(parsers));

            return AllParser.Parser(parsers);
        }

        /// <summary>
        /// Returns a parser which tries to parse all of the <paramref name="parsers"/> in a sequence.
        /// If it succeeds it returns a list of the parsed results.
        /// If it fails it returns the first encountered parse error
        /// </summary>
        /// <typeparam name="T"> The type of parsers </typeparam>
        /// <param name="parsers"> Parsers to sequentially apply </param>
        /// <returns> Parser which sequentially aplies all of the given parsers </returns>
        /// <exception cref="ArgumentNullException"> If parser array is null </exception>
        public static Parser<IReadOnlyList<T>, TInputToken> All<T, TInputToken>(
            IEnumerable<Parser<T, TInputToken>> parsers
        )
        {
            if (parsers is null) throw new ArgumentNullException(nameof(parsers));

            return AllParser.Parser(parsers);
        }
    }
}
