using System;
using System.Linq.Expressions;
using System.Linq;

namespace ParsecCore.MaybeNS
{
    /// <summary>
    /// Represents a valid <see cref="IMaybe{T}"/> value
    /// </summary>
    /// <typeparam name="T"> The type of value held </typeparam>
    internal struct Just<T> : IMaybe<T>, IEquatable<Just<T>>
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

        public override bool Equals(object? obj)
        {
            return obj is Just<T> other && Equals(other);
        }

        public bool Equals(Just<T> other)
        {
            return _value != null && _value.Equals(_value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        private readonly T _value;
    }
}
