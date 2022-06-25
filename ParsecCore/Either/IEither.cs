using System;

namespace ParsecCore.EitherNS
{
    public interface IEither<out TLeft, out TRight>
    {
        /// <summary>
        /// Takes the IEither objects and maps its value according to the given function
        /// and the value which it contains
        /// </summary>
        /// <typeparam name="TNewLeft">The new type of value on the left</typeparam>
        /// <typeparam name="TNewRight">The new type of value on the right</typeparam>
        /// <param name="right">The mapping function for if the IEither contains a right value</param>
        /// <param name="left">The mapping function for if the IEither contains a left value</param>
        /// <returns>A new IEither mapped according to the supplied functions</returns>
        IEither<TNewLeft, TNewRight> Map<TNewLeft, TNewRight>(Func<TRight, TNewRight> right, Func<TLeft, TNewLeft> left);
        IEither<TLeft, TNewRight> Map<TNewRight>(Func<TRight, TNewRight> right);

        T Match<T>(Func<TRight, T> right, Func<T> left);

        /// <summary>
        /// Whether this IEither is holding the LEFT value
        /// </summary>
        public bool HasLeft { get; }
        /// <summary>
        /// Whether this IEither is holding the RIGHT value
        /// </summary>
        public bool HasRight { get; }

        /// <summary>
        /// Returns the left value if the IEither is holding it.
        /// Otherwise throws InvalidOperationException
        /// </summary>
        public TLeft Left { get; }
        /// <summary>
        /// Returns the right value if the IEither is holding it.
        /// Otherwise throws InvalidOperationException
        /// </summary>
        public TRight Right { get; }
    }
}
