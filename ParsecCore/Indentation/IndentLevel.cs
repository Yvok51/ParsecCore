using ParsecCore.Input;
using System;

namespace ParsecCore.Indentation
{
    public readonly struct IndentLevel : IComparable<IndentLevel>, IComparable, IEquatable<IndentLevel>
    {
        public IndentLevel(int indentation)
        {
            Indentation = indentation;
        }

        public IndentLevel(Position position)
        {
            Indentation = position.Column - 1;
        }

        public readonly int Indentation { get; init; }

        public static readonly IndentLevel FirstPosition = (IndentLevel)Position.BeginningColumn;

        public override string ToString()
        {
            return Indentation.ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj is IndentLevel indentation && Equals(indentation);
        }

        public override int GetHashCode()
        {
            return Indentation.GetHashCode();
        }

        public int CompareTo(IndentLevel other)
        {
            return Indentation.CompareTo(other.Indentation);
        }

        public int CompareTo(object? obj)
        {
            return Indentation.CompareTo(obj);
        }

        public bool Equals(IndentLevel other)
        {
            return Indentation.Equals(other.Indentation);
        }

        public static bool operator ==(IndentLevel left, IndentLevel right) => left.Equals(right);
        public static bool operator !=(IndentLevel left, IndentLevel right) => !left.Equals(right);

        public static bool operator <(IndentLevel left, IndentLevel right) => left.Indentation < right.Indentation;
        public static bool operator >(IndentLevel left, IndentLevel right) => left.Indentation > right.Indentation;
        public static bool operator <=(IndentLevel left, IndentLevel right) => left.Indentation <= right.Indentation;
        public static bool operator >=(IndentLevel left, IndentLevel right) => left.Indentation >= right.Indentation;

        public static explicit operator int(IndentLevel indentation) => indentation.Indentation;
        public static explicit operator IndentLevel(int indentation) => new(indentation);

        public static explicit operator IndentLevel(Position position) => new(position.Column - 1);

    }
}
