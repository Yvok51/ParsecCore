using System;

namespace ParsecCore
{
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
    }
}
