using System;

namespace ParsecCore
{
    /// <summary>
    /// The implementation of the failed parse case of the <see cref="IResult{T, TInput}"/> interface.
    /// </summary>
    /// <typeparam name="T"> The output type of the result </typeparam>
    /// <typeparam name="TInput"> The input symbol type </typeparam>
    internal class Failure<T, TInput> : IResult<T, TInput>
    {
        public Failure(ParseError error, IParserInput<TInput> unconsumedInput)
        {
            Error = error;
            UnconsumedInput = unconsumedInput;
        }

        public IParserInput<TInput> UnconsumedInput { get; init; }

        public bool IsError => true;

        public bool IsResult => false;

        public ParseError Error { get; init; }

        public T Result => throw new InvalidOperationException("Holds an error");

        public override string? ToString()
        {
            return Error.ToString();
        }
    }
}
