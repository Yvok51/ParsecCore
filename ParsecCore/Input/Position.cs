namespace ParsecCore.Input
{
    struct Position
    {
        public int Line { get; init; }
        public int Column { get; init; }

        public override string ToString()
        {
            return $"{Line}:{Column}";
        }
    }
}
