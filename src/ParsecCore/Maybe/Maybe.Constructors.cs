namespace ParsecCore.MaybeNS
{
    public partial struct Maybe
    {
        /// <summary>
        /// Create a valid <see cref="Maybe{T}"/> value
        /// </summary>
        /// <typeparam name="T"> The type of value </typeparam>
        /// <param name="value"> The value whose <see cref="Maybe{T}"/> to create </param>
        /// <returns> A valid case of the <see cref="Maybe{T}"/> </returns>
        public static Maybe<T> FromValue<T>(T value)
        {
            return new Maybe<T>(value);
        }

        /// <summary>
        /// Creates an invalid case of the <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="T"> The type of the value if it was valid </typeparam>
        /// <returns> An invalid <see cref="Maybe{T}"/> value </returns>
        public static Maybe<T> Nothing<T>()
        {
            return new Maybe<T>();
        }
    }
}
