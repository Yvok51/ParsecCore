namespace ParsecCore.EitherNS
{
    internal static class Either
    {
        /// <summary>
        /// Returns IEither representing a success holding the given value
        /// </summary>
        /// <typeparam name="TError"> The type of the left (error) value </typeparam>
        /// <typeparam name="TResult"> The type of the right (success) value - this is the one we are holding </typeparam>
        /// <param name="value"> The value the returning IEIther will hold </param>
        /// <returns> IEither which represents a success, holding the value `value` </returns>
        public static IEither<TError, TResult> Result<TError, TResult>(TResult value)
        {
            return new ResultValue<TError, TResult>(value);
        }

        /// <summary>
        /// Returns IEither representing an error holding the given error
        /// </summary>
        /// <typeparam name="TError"> The type of the left (error) value - this is the one we are holding </typeparam>
        /// <typeparam name="TResult"> The type of the right (success) value </typeparam>
        /// <param name="error"> The error the returning IEIther will hold </param>
        /// <returns> IEither which represents an error, holding the value `error` </returns>
        public static IEither<TError, TResult> Error<TError, TResult>(TError error)
        {
            return new ErrorValue<TError, TResult>(error);
        }

        /// <summary>
        /// Change the result type of an error.
        /// Used instead of calling <c>Either.Error<TError, TNewResult>(either.Error)</c>.
        /// </summary>
        /// <typeparam name="TError"> The type of the left (error) value </typeparam>
        /// <typeparam name="TNewResult"> The old type of the right (success) value </typeparam>
        /// <typeparam name="TResult"> The new type of the right (success) value </typeparam>
        /// <param name="either"> The either to retype </param>
        /// <returns> The same error only with different type </returns>
        public static IEither<TError, TNewResult> RetypeError<TError, TResult, TNewResult>(
            IEither<TError, TResult> either
        )
        {
            return new ErrorValue<TError, TNewResult>(either.Error);
        }
    }
}
