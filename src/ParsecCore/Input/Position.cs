using System;

namespace ParsecCore.Input
{
    public struct Position : IEquatable<Position>
    {
        /// <summary>
        /// Get the default starting position
        /// </summary>
        /// <param name="offset"> Specify the offset the starting position starts at. By default 1 </param>
        /// <returns> The default starting position </returns>
        public static Position Start(int offset = 0) => new Position(line: 1, column: BeginningColumn, offset: offset);

        public Position(int line, int column, int offset)
        {
            Line = line;
            Column = column;
            Offset = offset;
        }

        public int Line { get; init; }
        public int Column { get; init; }
        internal int Offset { get; init; }

        public static readonly int BeginningColumn = 1;

        /// <summary>
        /// Forward the position by a given amount of characters in the same row as before
        /// </summary>
        /// <param name="columnIncrease"> By how many characters to move forward </param>
        /// <returns> The forwarded position </returns>
        public Position WithIncreasedColumn(int columnIncrease = 1)
        {
            return new Position(line: Line, column: Column + columnIncrease, offset: Offset);
        }

        /// <summary>
        /// Forward the position by a given amount of characters in the same row as before.
        /// Increases the column to the nearest tab position.
        /// For example, if we are at column 1 and the <paramref name="tabSize"/> is 4, then the final column is 5.
        /// However, if we are at column 3 and the <paramref name="tabSize"/> is again 4, the the final column is again 5.
        /// </summary>
        /// <param name="columnIncrease"> By how many characters to move forward </param>
        /// <returns> The forwarded position </returns>
        public Position WithTab(int tabSize)
        {
            return new Position(line: Line, column: Column + tabSize - ((Column - 1) % tabSize), offset: Offset);
        }

        /// <summary>
        /// Forward the position to subsequent line/row
        /// </summary>
        /// <param name="lineIncrease"> By how many lines to move forward </param>
        /// <returns> The forwared position </returns>
        public Position WithNewLine(int lineIncrease = 1)
        {
            return new Position(line: Line + lineIncrease, column: BeginningColumn, offset: Offset);
        }

        /// <summary>
        /// Increase the amount of read tokens (bytes, ...) from the input stream
        /// </summary>
        /// <param name="offsetBy"> How much to offset by (how many bytes the character took up) </param>
        /// <returns> The forwared position </returns>
        public Position WithIncreasedOffset(int offsetBy = 1)
        {
            return new Position(line: Line, column: Column, offset: Offset + offsetBy);
        }

        public override string ToString()
        {
            return $"{Line}:{Column}";
        }

        public override bool Equals(object? obj) =>
            !(obj is null) && obj is Position other && Equals(other);

        public bool Equals(Position other) =>
            other.Offset == Offset && other.Line == Line && other.Column == Column;

        public override int GetHashCode() =>
            HashCode.Combine(Line, Column, Offset);

        public static bool operator ==(Position left, Position right) =>
            left.Equals(right);

        public static bool operator !=(Position left, Position right) =>
            !left.Equals(right);

        public static bool operator <(Position left, Position right) =>
            left.Offset < right.Offset;

        public static bool operator >(Position left, Position right) =>
            left.Offset > right.Offset;

        /// <summary>
        /// Returns the minimum position based on the offset number
        /// </summary>
        /// <param name="left"> The first position to consider </param>
        /// <param name="right"> The second position to consider </param>
        /// <returns> The minimum of the two positions </returns>
        public static Position Min(Position left, Position right)
        {
            return right < left ? right : left;
        }
    }
}
