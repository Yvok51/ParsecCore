using System;

namespace ParsecCore.Either
{
    struct Result<TError, TResult> : IEither<TError, TResult>
    {
        public Result(TResult result) 
        {
            _result = result;
        }

        public bool HasLeft => false;

        public bool HasRight => true;

        public TError Left => throw new InvalidOperationException();

        public TResult Right => _result;

        public IEither<TNewLeft, TNewRight> Map<TNewLeft, TNewRight>(Func<TResult, TNewRight> right, Func<TError, TNewLeft> left)
        {
            return new Result<TNewLeft, TNewRight>(right(_result));
        }

        public IEither<TError, TNewRight> Map<TNewRight>(Func<TResult, TNewRight> right)
        {
            return new Result<TError, TNewRight>(right(_result));
        }

        public T Match<T>(Func<TResult, T> right, Func<T> left)
        {
            return right(_result);
        }

        private readonly TResult _result;
    }
}
