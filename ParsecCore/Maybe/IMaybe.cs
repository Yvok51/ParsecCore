using System;

namespace ParsecCore.MaybeNS
{
    public interface IMaybe<T>
    {
        /// <summary>
        /// Answers whether the object represents a value or is empty
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Get the stored value.
        /// Throws <see cref="InvalidOperationException"/> if the <c>IMaybe</c>
        /// is empty.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Maps the stored value if it is present
        /// </summary>
        /// <typeparam name="TNew"> The type of the new stored value </typeparam>
        /// <param name="map"> The function to map the value </param>
        /// <returns> <c>IMaybe</c> with the new mapped value </returns>
        IMaybe<TNew> Map<TNew>(Func<T, TNew> map);

        /// <summary>
        /// Returns a mapped value
        /// </summary>
        /// <typeparam name="TNew"> The type of the mapped stored value </typeparam>
        /// <param name="just"> Mapping function to apply if value is present </param>
        /// <param name="nothing"> Fucntion to use if the instance is empty </param>
        /// <returns> Mapped value stored inside </returns>
        TNew Match<TNew>(Func<T, TNew> just, Func<TNew> nothing);
    }
}
