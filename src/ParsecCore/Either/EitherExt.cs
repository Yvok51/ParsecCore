using System;

namespace ParsecCore.EitherNS
{
    internal static class EitherExt
    {
        /// <summary>
        /// Select is an exptension method used in LINQ expressions for a single `from ... in ...` statement
        /// In context of <see cref="IEither{TLeft, TRight}"/> we use it to simulate 
        /// the list comprehension notation (weaker do notation) of Haskell. 
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
        /// In context of <see cref="IEither{TLeft, TRight}"/> we use it to simulate 
        /// the list comprehension notation (weaker do notation) of Haskell.
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
                firstResult =>
                {
                    var second = getSecond(firstResult);
                    return second.Match(
                        secondResult => Either.Result<TLeft, TResult>(getResult(firstResult, secondResult)),
                        () => Either.Error<TLeft, TResult>(second.Error)
                    );
                },
                () => Either.Error<TLeft, TResult>(first.Error)
            );
        }

        /// <summary>
        /// Combines two errors together according to the method <see cref="ParseError.Accept(ParseError)"/>.
        /// Presumes both <c>IEither</c> are errors, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        /// <typeparam name="T">  The result type of the either </typeparam>
        /// <param name="left"> The first <c>IEither</c> </param>
        /// <param name="right"> The second <c>IEither</c> </param>
        /// <returns> Error combining both input errors </returns>
        public static IEither<ParseError, T> CombineErrors<T>(
            this IEither<ParseError, T> left,
            IEither<ParseError, T> right
        )
        {
            return Either.Error<ParseError, T>(left.Error.Accept(right.Error, None.Instance));
        }

    }
}
