namespace ParsecCore
{
    public static class Maybe
    {
        /// <summary>
        /// Create a <see cref="Maybe{T}"/> that contains a value
        /// </summary>
        /// <typeparam name="T"> The type of value </typeparam>
        /// <param name="value"> The value to wrap <see cref="Maybe{T}"/> over </param>
        /// <returns> A nonempty case of the <see cref="Maybe{T}"/> </returns>
        public static Maybe<T> FromValue<T>(T value)
        {
            return new Maybe<T>(value);
        }

        /// <summary>
        /// Creates an empty case of the <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="T"> The type of the value if it was contained by <see cref="Maybe{T}"/> </typeparam>
        /// <returns> An empty <see cref="Maybe{T}"/> </returns>
        public static Maybe<T> Nothing<T>()
        {
            return new Maybe<T>();
        }
    }
}
