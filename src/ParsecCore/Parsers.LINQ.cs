using ParsecCore.Input;
using System;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Extension method enabling us to use the LINQ syntax for the parsers.
        /// The LINQ syntax imitates 'do' notation found in haskell.
        /// <c>for x in xs</c> is equivalent to <c>x <- xs</c> and <c>select x</c> is equivalent to <c>return x</c>
        /// This method is therefore similar to <c>bind</c> <c>(>>=)</c>
        /// </summary>
        /// <typeparam name="TSource"> > The type of the source parser </typeparam>
        /// <typeparam name="TResult"> The type of the resulting parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The source parser </param>
        /// <param name="projection"> The funtion to map the result of the source parser with </param>
        /// <returns> Parser which maps the result of the source parser to a new value </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<TResult, TInputToken> Select<TSource, TResult, TInputToken>(
            this Parser<TSource, TInputToken> parser,
            Func<TSource, TResult> projection
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (projection is null) throw new ArgumentNullException(nameof(projection));

            return (input) =>
            {
                var result = parser(input);
                if (result.IsError)
                {
                    return Result.RetypeError<TSource, TResult, TInputToken>(result);
                }
                else
                {
                    return Result.Success(projection(result.Result), result.UnconsumedInput);
                }
            };
        }

        /// <summary>
        /// Extension method enabling us to use the LINQ syntax for the parsers.
        /// The LINQ syntax imitates 'do' notation found in haskell.
        /// <c>for x in xs</c> is equivalent to <c>x <- xs</c> and <c>select x</c> is equivalent to <c>return x</c>
        /// This method is therefore similar to <c>bind</c> <c>(>>=)</c>
        /// </summary>
        /// <typeparam name="TFirst"> The type of the source parser </typeparam>
        /// <typeparam name="TSecond"> The type of the parser returned by the chained method </typeparam>
        /// <typeparam name="TResult"> The type of the resulting parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="first"> The source parser </param>
        /// <param name="getSecond"> The function to chain to the result of the source parser </param>
        /// <param name="getResult"> Callback which combines the two results together </param>
        /// <returns> 
        /// Parser which performs the source parser and afterwards the chained method in that order
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<TResult, TInputToken> SelectMany<TFirst, TSecond, TResult, TInputToken>(
            this Parser<TFirst, TInputToken> first,
            Func<TFirst, Parser<TSecond, TInputToken>> getSecond,
            Func<TFirst, TSecond, TResult> getResult
        )
        {
            if (first is null) throw new ArgumentNullException(nameof(first));
            if (getSecond is null) throw new ArgumentNullException(nameof(getSecond));
            if (getResult is null) throw new ArgumentNullException(nameof(getResult));

            return (input) =>
            {
                var firstResult = first(input);
                if (firstResult.IsError)
                {
                    return Result.RetypeError<TFirst, TResult, TInputToken>(firstResult);
                }

                var secondResult = getSecond(firstResult.Result)(firstResult.UnconsumedInput);
                if (secondResult.IsError)
                {
                    return Result.RetypeError<TSecond, TResult, TInputToken>(secondResult);
                }

                return Result.Success(
                    getResult(firstResult.Result, secondResult.Result),
                    secondResult.UnconsumedInput
                );
            };
        }

        /// <summary>
        /// Extension method enabling us to use the <code>where</code> clause in LINQ syntax.
        /// In case the predicate fails then a simple <code>"Assertion failed"</code> message is reported.
        /// It is therefore recommended to use 
        /// <see cref="Assert{T, TInputToken}(Parser{T, TInputToken}, Func{T, bool}, string)"/> or
        /// <see cref="Assert{T, TInputToken}(Parser{T, TInputToken}, Func{T, bool}, Func{T, Position, ParseError})"/>
        /// instead.
        /// </summary>
        /// <typeparam name="T"> The result type of the parser </typeparam>
        /// <typeparam name="TInputToken"> The input token of the parser </typeparam>
        /// <param name="parser"> Parser whose result to test </param>
        /// <param name="predicate"> Predicate to test the result of the parse with </param>
        /// <returns>
        /// Parser which tests its result against a predicate and in case the result does not pass it fails
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<T, TInputToken> Where<T, TInputToken>(
            this Parser<T, TInputToken> parser,
            Func<T, bool> predicate
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            return parser.Assert(predicate, "Assertion failed");
        }
    }
}
