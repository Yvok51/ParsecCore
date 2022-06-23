namespace ParsecCore.Input
{
    public struct Position
    {
        public static Position Start = new Position(line: 1, column: 1, offset: 0);
        
        private Position(int line, int column, int offset)
        {
            Line = line;
            Column = column;
            Offset = offset;
        }

        public int Line { get; init; }
        public int Column { get; init; }
        public int Offset { get; init; }

        public Position NextColumn()
        {
            return new Position(line: Line, column: Column + 1, offset: Offset + 1);
        }

        public Position NextLine()
        {
            return new Position(line: Line + 1, column: 1, offset: Offset + 1);
        }

        public override string ToString()
        {
            return $"{Line}:{Column}";
        }
    }
}
