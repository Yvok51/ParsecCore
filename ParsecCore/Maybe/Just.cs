using System;

namespace ParsecCore.MaybeNS
{
    internal struct Just<T> : IMaybe<T>
    {
        public Just(T value)
        {
            _value = value;
        }

        public bool IsEmpty => false;

        public T Value => _value;

        public IMaybe<TNew> Map<TNew>(Func<T, TNew> map)
        {
            return new Just<TNew>(map(_value));
        }

        public TNew Match<TNew>(Func<T, TNew> just, Func<TNew> nothing)
        {
            return just(_value);
        }

        private readonly T _value;
    }
}
