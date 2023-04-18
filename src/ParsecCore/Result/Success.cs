using System;

namespace ParsecCore
{
    internal class Success<T, TInput> : IResult<T, TInput>
    {
        public Success(T value, IParserInput<TInput> unconsumedInput)
        {
            Result = value;
            UnconsumedInput = unconsumedInput;
        }

        public IParserInput<TInput> UnconsumedInput { get; init; }

        public bool IsError => false;

        public bool IsResult => true;

        public ParseError Error => throw new InvalidOperationException("Holds a result");

        public T Result { get; init; }

        public override string ToString()
        {
            return Result.ToString();
        }
    }
}
