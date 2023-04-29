using System;

namespace ParsecCore
{
    /// <summary>
    /// The result of a parse.
    /// Could be either a failed parse and contain an error or a successful and contain an output.
    /// The successful parse can also contain the most specific error that ever occured during the parsing
    /// </summary>
    /// <typeparam name="T"> The output type of the result </typeparam>
    /// <typeparam name="TInput"> The input symbol type </typeparam>
    public interface IResult<out T, TInput>
    {
        IParserInput<TInput> UnconsumedInput { get; }

        /// <summary>
        /// Whether this <see cref="IResult{T, TInput}"/> is holding the error value
        /// </summary>
        public bool IsError { get; }
        /// <summary>
        /// Whether this <see cref="IResult{T, TInput}"/> is holding the result value
        /// </summary>
        public bool IsResult { get; }

        /// <summary>
        /// Returns the error value if the <see cref="IResult{T, TInput}"/> is holding it.
        /// Otherwise throws <see cref="InvalidOperationException"/>
        /// </summary>
        public ParseError? Error { get; }
        /// <summary>
        /// Returns the result value if the <see cref="IResult{T, TInput}"/> is holding it.
        /// Otherwise throws <see cref="InvalidOperationException"/>
        /// </summary>
        public T Result { get; }
    }
}
