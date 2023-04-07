using ParsecCore.Input;
using System;

namespace ParsecCore
{
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
        public ParseError Error { get; }
        /// <summary>
        /// Returns the result value if the <see cref="IResult{T, TInput}"/> is holding it.
        /// Otherwise throws <see cref="InvalidOperationException"/>
        /// </summary>
        public T Result { get; }
    }
}
