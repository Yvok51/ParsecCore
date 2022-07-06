using System;

namespace ParsecCore.EitherNS
{
    struct ResultValue<TError, TResult> : IEither<TError, TResult>
    {
        public ResultValue(TResult result)
        {
            _result = result;
        }

        public bool IsError => false;

        public bool IsResult => true;

        public TError Error => throw new InvalidOperationException();

        public TResult Result => _result;

        public IEither<TNewLeft, TNewRight> Map<TNewLeft, TNewRight>(Func<TResult, TNewRight> right, Func<TError, TNewLeft> left)
        {
            return new ResultValue<TNewLeft, TNewRight>(right(_result));
        }

        public IEither<TError, TNewRight> Map<TNewRight>(Func<TResult, TNewRight> right)
        {
            return new ResultValue<TError, TNewRight>(right(_result));
        }

        public T Match<T>(Func<TResult, T> right, Func<T> left)
        {
            return right(_result);
        }

        public override string ToString()
        {
            return _result is null ? "null" : _result.ToString();
        }

        private readonly TResult _result;
    }
}
