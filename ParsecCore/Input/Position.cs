using System;

namespace ParsecCore.Input
{
    public struct Position : IEquatable<Position>
    {
        public static Position Start(int offset = 0) => new Position(line: 1, column: 1, offset: offset);
        
        public Position(int line, int column, int offset)
        {
            Line = line;
            Column = column;
            Offset = offset;
        }

        public int Line { get; init; }
        public int Column { get; init; }
        public int Offset { get; init; }

        public Position NextColumn(int offsetBy = 1)
        {
            return new Position(line: Line, column: Column + 1, offset: Offset + offsetBy);
        }

        public Position NextLine(int offsetBy = 1)
        {
            return new Position(line: Line + 1, column: 1, offset: Offset + offsetBy);
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
    }
}
