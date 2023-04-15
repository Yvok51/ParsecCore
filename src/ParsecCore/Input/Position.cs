using System;

namespace ParsecCore
{
    public struct Position : IEquatable<Position>
    {

        public static readonly int BeginningColumn = 1;

        /// <summary>
        /// Get the default starting position
        /// </summary>
        /// <param name="offset"> Specify the offset the starting position starts at. By default 1 </param>
        /// <returns> The default starting position </returns>
        public static readonly Position Start = new Position(line: 1, column: BeginningColumn);

        public Position(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; init; }
        public int Column { get; init; }

        /// <summary>
        /// Forward the position by a given amount of characters in the same row as before
        /// </summary>
        /// <param name="columnIncrease"> By how many characters to move forward </param>
        /// <returns> The forwarded position </returns>
        public Position WithIncreasedColumn(int columnIncrease = 1)
        {
            return new Position(line: Line, column: Column + columnIncrease);
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
            return new Position(line: Line, column: Column + tabSize - ((Column - 1) % tabSize));
        }

        /// <summary>
        /// Forward the position to subsequent line/row
        /// </summary>
        /// <param name="lineIncrease"> By how many lines to move forward </param>
        /// <returns> The forwared position </returns>
        public Position WithNewLine(int lineIncrease = 1)
        {
            return new Position(line: Line + lineIncrease, column: BeginningColumn);
        }

        public override string ToString()
        {
            return $"{Line}:{Column}";
        }

        public override bool Equals(object? obj) =>
            !(obj is null) && obj is Position other && Equals(other);

        public bool Equals(Position other) =>
            other.Line == Line && other.Column == Column;

        public override int GetHashCode() =>
            HashCode.Combine(Line, Column);

        public static bool operator ==(Position left, Position right) =>
            left.Line == right.Line && left.Column == right.Column;
        public static bool operator !=(Position left, Position right) =>
            left.Line != right.Line || left.Column != right.Column;

        public static bool operator <(Position left, Position right) =>
            left.Line < right.Line || (left.Line == right.Line && left.Column < right.Column);

        public static bool operator >(Position left, Position right) =>
            left.Line > right.Line || (left.Line == right.Line && left.Column > right.Column);

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
