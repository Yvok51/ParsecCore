using System;

namespace ParsecCore.Indentation
{
    /// <summary>
    /// Represents a level of indentation - the column to which we re indented.
    /// </summary>
    public readonly struct IndentLevel : IComparable<IndentLevel>, IComparable, IEquatable<IndentLevel>
    {
        /// <summary>
        /// Creates a new <see cref="IndentLevel"/> and sets it column to the specified value.
        /// Note <see cref="IndentLevel.Indentation"/> is one less then the column.
        /// </summary>
        /// <param name="column"> The column we are indented to </param>
        public IndentLevel(int column)
        {
            Column = column;
        }

        /// <summary>
        /// The column we are located at.
        /// </summary>
        public readonly int Column { get; init; }

        /// <summary>
        /// The indentation we are at.
        /// For example, if we are at the first column then the indentation is zero
        /// </summary>
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
