using System;

namespace ParsecCore.Either
{
    static class EitherExt
    {
        public static IEither<TLeft, TRight> Result<TLeft, TRight>(TRight value) 
        {
            return new Result<TLeft, TRight>(value);
        }

        public static IEither<TLeft, TRight> Error<TLeft, TRight>(TLeft value)
        {
            return new Error<TLeft, TRight>(value);
        }

        /// <summary>
        /// Select is an exptension method used in LINQ expressions for a single `from ... in ...` statement
        /// In context of IEither we use it to simulate the do notation of Haskell
        /// Inspired by this blog post: https://tyrrrz.me/blog/monadic-comprehension-via-linq
        /// </summary>
        /// <typeparam name="TLeft"> The left (error) type </typeparam>
        /// <typeparam name="TSource"> The result type of the source IEither object </typeparam>
        /// <typeparam name="TResult"> The result type of the projected IEither object </typeparam>
        /// <param name="either"> The source IEither object </param>
        /// <param name="projection"> The function with which project the result value from TSource to TResult type </param>
        /// <returns> An IEither object with the result value projected according to the function prejection </returns>
        public static IEither<TLeft, TResult> Select<TLeft, TSource, TResult>(
            this IEither<TLeft, TSource> either,
            Func<TSource, TResult> projection
        )
        {
            return either.Map(
                sourceValue => projection(sourceValue)
            );
        }

        /// <summary>
        /// Select many is an extension method used in LINQ expressions to chain multiple `from ... in ...` statements together
        /// In context of IEither we use it to simulate the do notation of Haskell
        /// Inspired by this blog post: https://tyrrrz.me/blog/monadic-comprehension-via-linq
        /// </summary>
        /// <typeparam name="TLeft"> The left (error) type - it is consistent throughout </typeparam>
        /// <typeparam name="TFirst"> The result value of the input IEither </typeparam>
        /// <typeparam name="TSecond"> The result value of the IEither we are chaining after the input IEither </typeparam>
        /// <typeparam name="TResult"> The final result type that we get from the combining function </typeparam>
        /// <param name="first"> The input IEither we call the method on </param>
        /// <param name="getSecond"> Delegate which gives us the IEither that is chained after the first IEither </param>
        /// <param name="getResult"> The delegate used to combine the results </param>
        /// <returns> IEither with the final result, or an error </returns>
        public static IEither<TLeft, TResult> SelectMany<TLeft, TFirst, TSecond, TResult>(
            this IEither<TLeft, TFirst> first,
            Func<TFirst, IEither<TLeft, TSecond>> getSecond,
            Func<TFirst, TSecond, TResult> getResult)
        {
            return first.Match(
                firstResult => {
                    var second = getSecond(firstResult);
                    return second.Match(
                        secondResult => Result<TLeft, TResult>(getResult(firstResult, secondResult)),
                        () => Error<TLeft, TResult>(second.Left)
                    );
                },
                () => Error<TLeft, TResult>(first.Left)
            );
        }

    }
}
