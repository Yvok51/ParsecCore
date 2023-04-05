using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ParsecCore.MaybeNS
{
    public partial struct Maybe<T> : IEquatable<Maybe<T>>
    {
        public Maybe(T value)
        {
            IsEmpty = false;
            _value = value;
        }

        public Maybe()
        {
            IsEmpty = true;
            _value = default;
        }

        public T Value { get { if (IsEmpty) throw new InvalidOperationException("Maybe is empty"); return _value; } }
        public bool IsEmpty { get; init; }
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
