using System;

namespace ParsecCore.MaybeNS
{
    /// <summary>
    /// Represents the invalid <see cref="IMaybe{T}"/> value
    /// </summary>
    /// <typeparam name="T"> The type of value that would be held, if the value was valid </typeparam>
    internal struct Nothing<T> : IMaybe<T>
    {
        public bool IsEmpty => true;

        public T Value => throw new InvalidOperationException();

        public IMaybe<TNew> Map<TNew>(Func<T, TNew> map)
        {
            return new Nothing<TNew>();
        }

        public TNew Match<TNew>(Func<T, TNew> just, Func<TNew> nothing)
        {
            return nothing();
        }
    }
}
