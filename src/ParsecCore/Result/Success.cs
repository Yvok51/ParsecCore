using System;

namespace ParsecCore
{
    /// <summary>
    /// The implementation of the successful parse case of the <see cref="IResult{T, TInput}"/> interface.
    /// </summary>
    /// <typeparam name="T"> The output type of the result </typeparam>
    /// <typeparam name="TInput"> The input symbol type </typeparam>
    internal class Success<T, TInput> : IResult<T, TInput>
    {
        public Success(T value, ParseError? oldError, IParserInput<TInput> unconsumedInput)
        {
            Result = value;
            UnconsumedInput = unconsumedInput;
            Error = oldError;
        }

        public IParserInput<TInput> UnconsumedInput { get; init; }

        public bool IsError => false;

        public bool IsResult => true;

        public ParseError? Error { get; init; }

        public T Result { get; init; }

        public override string ToString()
        {
            return Result!.ToString()!;
        }
    }
}
