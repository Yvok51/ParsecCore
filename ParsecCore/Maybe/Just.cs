using System;

namespace ParsecCore.MaybeNS
{
    /// <summary>
    /// Represents a valid <see cref="IMaybe{T}"/> value
    /// </summary>
    /// <typeparam name="T"> The type of value held </typeparam>
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
