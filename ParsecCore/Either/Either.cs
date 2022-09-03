namespace ParsecCore.EitherNS
{
    internal static class Either
    {
        /// <summary>
        /// Returns IEither representing a success holding the given value
        /// </summary>
        /// <typeparam name="TLeft"> The type of the left (error) value </typeparam>
        /// <typeparam name="TRight"> The type of the right (success) value - this is the one we are holding </typeparam>
        /// <param name="value"> The value the returning IEIther will hold </param>
        /// <returns> IEither which represents a success, holding the value `value` </returns>
        public static IEither<TLeft, TRight> Result<TLeft, TRight>(TRight value)
        {
            return new ResultValue<TLeft, TRight>(value);
        }

        /// <summary>
        /// Returns IEither representing an error holding the given error
        /// </summary>
        /// <typeparam name="TLeft"> The type of the left (error) value - this is the one we are holding </typeparam>
        /// <typeparam name="TRight"> The type of the right (success) value </typeparam>
        /// <param name="error"> The error the returning IEIther will hold </param>
        /// <returns> IEither which represents an error, holding the value `error` </returns>
        public static IEither<TLeft, TRight> Error<TLeft, TRight>(TLeft error)
        {
            return new ErrorValue<TLeft, TRight>(error);
        }
    }
}
