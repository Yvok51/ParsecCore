using System;

namespace ParsecCore.EitherNS
{
    public interface IEither<out TLeft, out TRight>
    {
        /// <summary>
        /// Takes the IEither objects and maps its value according to the given function
        /// and the value which it contains
        /// </summary>
        /// <typeparam name="TNewError"> The new type of value for the error </typeparam>
        /// <typeparam name="TNewResult"> The new type of value for the result </typeparam>
        /// <param name="resultMap"> The mapping function for if the IEither contains a result value </param>
        /// <param name="errorMap"> The mapping function for if the IEither contains a error value </param>
        /// <returns> A new IEither mapped according to the supplied functions </returns>
        IEither<TNewError, TNewResult> Map<TNewError, TNewResult>(Func<TRight, TNewResult> resultMap, Func<TLeft, TNewError> errorMap);

        /// <summary>
        /// Same as <see cref="Map{TNewError, TNewResult}(Func{TRight, TNewResult}, Func{TLeft, TNewError})"/>
        /// but only for the result value, if the object contains it. The left value remains the same
        /// equivalent to <c>Map(resultMap, (x) => x)</c>
        /// </summary>
        /// <typeparam name="TNewRight"> The new type of value for the result </typeparam>
        /// <param name="resultMap"> The mapping function for if the IEither contains a result value </param>
        /// <returns></returns>
        IEither<TLeft, TNewRight> Map<TNewRight>(Func<TRight, TNewRight> resultMap);

        /// <summary>
        /// Unwrap the value inside IEither.
        /// Apply one of the supplied functions based on the contents of IEither
        /// and return the result.
        /// </summary>
        /// <typeparam name="T"> The type to unwrap to </typeparam>
        /// <param name="right"> The function to transform the result value </param>
        /// <param name="left"> Function to supply the value in case of error </param>
        /// <returns> The value calculated by one of the supplied functions </returns>
        T Match<T>(Func<TRight, T> right, Func<T> left);

        /// <summary>
        /// Whether this IEither is holding the ERROR - LEFT value
        /// </summary>
        public bool IsError { get; }
        /// <summary>
        /// Whether this IEither is holding the RESULT - RIGHT value
        /// </summary>
        public bool IsResult { get; }

        /// <summary>
        /// Returns the error - left value if the IEither is holding it.
        /// Otherwise throws InvalidOperationException
        /// </summary>
        public TLeft Error { get; }
        /// <summary>
        /// Returns the result - right value if the IEither is holding it.
        /// Otherwise throws InvalidOperationException
        /// </summary>
        public TRight Result { get; }
    }
}
