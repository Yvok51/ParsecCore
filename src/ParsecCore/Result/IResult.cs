using ParsecCore.EitherNS;
using ParsecCore.Input;

namespace ParsecCore
{
    public interface IResult<out T, TInput>
    {
        IParserInput<TInput> UnconsumedInput { get; }
        IEither<ParseError, T> ParseResult { get; }

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
        public ParseError Error { get; }
        /// <summary>
        /// Returns the result - right value if the IEither is holding it.
        /// Otherwise throws InvalidOperationException
        /// </summary>
        public T Result { get; }
    }
}
