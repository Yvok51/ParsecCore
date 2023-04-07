using ParsecCore.Input;
using System;

namespace ParsecCore
{
    internal class ResultImpl<T, TInput> : IResult<T, TInput>
    {
        public ResultImpl(T result, IParserInput<TInput> unconsumedInput)
        {
            UnconsumedInput = unconsumedInput;
            _result = result;
            _error = default;
            IsError = false;
        }

        public ResultImpl(ParseError error, IParserInput<TInput> unconsumedInput)
        {
            UnconsumedInput = unconsumedInput;
            _result = default;
            _error = error;
            IsError = false;
        }

        public IParserInput<TInput> UnconsumedInput { get; init; }

        public bool IsError { get; init; }

        public bool IsResult => !IsError;

        public ParseError Error
        {
            get
            {
                if (IsResult) throw new InvalidOperationException("Holds an error");
                return _error;
            }
        }

        T IResult<T, TInput>.Result
        {
            get
            {
                if (IsError) throw new InvalidOperationException("Holds a result");
                return _result;
            }
        }

        T? _result;
        ParseError? _error;
    }
}
