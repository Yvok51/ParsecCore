using ParsecCore.Input;

namespace ParsecCore
{
    public struct ParseError
    {
        public ParseError(string error, Position position)
        {
            Position = position;
            Error = error;
        }

        public Position Position { get; init; }

        public string Error { get; init; }

        public override string ToString()
        {
            return $"line {Position} - {Error}";
        }
    }
}
