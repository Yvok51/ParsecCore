namespace ParsecCore.MaybeNS
{
    internal static class Maybe
    {
        /// <summary>
        /// Create a valid <see cref="IMaybe{T}"/> value
        /// </summary>
        /// <typeparam name="T"> The type of value </typeparam>
        /// <param name="value"> The value whose <see cref="IMaybe{T}"/> to create </param>
        /// <returns> A valid case of the <see cref="IMaybe{T}"/> </returns>
        public static IMaybe<T> FromValue<T>(this T value)
        {
            return new Just<T>(value);
        }

        /// <summary>
        /// Creates an invalid case of the <see cref="IMaybe{T}"/>
        /// </summary>
        /// <typeparam name="T"> The type of the value if it was valid </typeparam>
        /// <returns> An invalid <see cref="IMaybe{T}"/> value </returns>
        public static IMaybe<T> Nothing<T>()
        {
            return new Nothing<T>();
        }
    }
}
