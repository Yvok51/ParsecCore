﻿using System;
using System.Linq.Expressions;
using System.Linq;

namespace ParsecCore.MaybeNS
{
    /// <summary>
    /// Represents the invalid <see cref="IMaybe{T}"/> value
    /// </summary>
    /// <typeparam name="T"> The type of value that would be held, if the value was valid </typeparam>
    internal struct Nothing<T> : IMaybe<T>, IEquatable<Nothing<T>>
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

        public override bool Equals(object? obj)
        {
            return obj is Nothing<T> other && Equals(other);
        }

        public bool Equals(Nothing<T> other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}