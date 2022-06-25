using System;

namespace ParsecCore.EitherNS
{
    struct Error<TError, TResult> : IEither<TError, TResult>
    {
        public Error(TError error) 
        {
            _error = error;
        }

        public bool HasLeft => true;

        public bool HasRight => false;

        public TError Left => _error;

        public TResult Right => throw new InvalidOperationException();

        public IEither<TError, TNewResult> Map<TNewResult>(Func<TResult, TNewResult> right)
        {
            return new Error<TError, TNewResult>(_error);
        }

        public IEither<TNewLeft, TNewRight> Map<TNewLeft, TNewRight>(Func<TResult, TNewRight> right, Func<TError, TNewLeft> left)
        {
            return new Error<TNewLeft, TNewRight>(left(_error));
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
