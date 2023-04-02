using ParsecCore.Input;
using System;

namespace ParsecCore.Indentation
{
    public readonly struct IndentLevel : IComparable<IndentLevel>, IComparable, IEquatable<IndentLevel>
    {
        public IndentLevel(int column)
        {
            Column = column;
        }

        public readonly int Column { get; init; }

        public readonly int Indentation { get => Column - 1; }

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
            return Column.GetHashCode();
        }

        public int CompareTo(IndentLevel other)
        {
            return Column.CompareTo(other.Column);
        }

        public int CompareTo(object? obj)
        {
            return Column.CompareTo(obj);
        }

        public bool Equals(IndentLevel other)
        {
            return Column.Equals(other.Column);
        }

        public static bool operator ==(IndentLevel left, IndentLevel right) => left.Equals(right);
        public static bool operator !=(IndentLevel left, IndentLevel right) => !left.Equals(right);

        public static bool operator <(IndentLevel left, IndentLevel right) => left.Column < right.Column;
        public static bool operator >(IndentLevel left, IndentLevel right) => left.Column > right.Column;
        public static bool operator <=(IndentLevel left, IndentLevel right) => left.Column <= right.Column;
        public static bool operator >=(IndentLevel left, IndentLevel right) => left.Column >= right.Column;

        public static explicit operator int(IndentLevel indentation) => indentation.Column;
        public static explicit operator IndentLevel(int indentation) => new(indentation);

        public static explicit operator IndentLevel(Position position) => new(position.Column);

    }
}
