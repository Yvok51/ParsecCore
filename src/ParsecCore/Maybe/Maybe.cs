using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ParsecCore.MaybeNS
{
    /// <summary>
    /// Represents a value that may not exist.
    /// Wrapper over a type that represents an optional value.
    /// </summary>
    /// <typeparam name="T"> The type of value that we are wrapping </typeparam>
    public struct Maybe<T> : IEquatable<Maybe<T>>
    {
        /// <summary>
        /// Creates a <see cref="Maybe{T}"/> that contains a value
        /// </summary>
        /// <param name="value"> value to wrap over </param>
        internal Maybe(T value)
        {
            IsEmpty = false;
            _value = value;
        }

        /// <summary>
        /// Create an empty <see cref="Maybe{T}"/>.
        /// Please use <see cref="Maybe.Nothing{T}"/> instead of this constructor.
        /// </summary>
        public Maybe()
        {
            IsEmpty = true;
            _value = default;
        }

        /// <summary>
        /// The value contained.
        /// If <see cref="Maybe{T}"/> is empty, then trows <see cref="InvalidOperationException"/>.
        /// </summary>
        public T Value { get { if (IsEmpty) throw new InvalidOperationException("Maybe is empty"); return _value!; } }
        /// <summary>
        /// Answers whether this <see cref="Maybe{T}"/> is empty - it does not contain a value
        /// </summary>
        public bool IsEmpty { get; init; }
        /// <summary>
        /// Answers whether this <see cref="Maybe{T}"/> contains a value.
        /// </summary>
        public bool HasValue { get => !IsEmpty; }

        private T? _value;

        public bool Equals(Maybe<T> other)
        {
            return (IsEmpty && other.IsEmpty)
                || (HasValue && other.HasValue && EqualityComparer<T>.Default.Equals(_value, other.Value));
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Maybe<T> maybe && Equals(maybe);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsEmpty, _value);
        }

        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !left.Equals(right);
        }
    }
}
