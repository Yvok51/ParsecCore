using System;

namespace ParsecCore.EitherNS
{
    internal struct ErrorValue<TError, TResult> : IEither<TError, TResult>
    {
        public ErrorValue(TError error)
        {
            _error = error;
        }

        public bool IsError => true;

        public bool IsResult => false;

        public TError Error => _error;

        public TResult Result => throw new InvalidOperationException();

        public IEither<TError, TNewResult> Map<TNewResult>(Func<TResult, TNewResult> right)
        {
            return new ErrorValue<TError, TNewResult>(_error);
        }

        public IEither<TNewLeft, TNewRight> Map<TNewLeft, TNewRight>(Func<TResult, TNewRight> right, Func<TError, TNewLeft> left)
        {
            return new ErrorValue<TNewLeft, TNewRight>(left(_error));
        }

        public T Match<T>(Func<TResult, T> right, Func<T> left)
        {
            return left();
        }

        public override string ToString()
        {
            return _error is null ? "null" : _error.ToString();
        }

        private readonly TError _error;
    }
}
