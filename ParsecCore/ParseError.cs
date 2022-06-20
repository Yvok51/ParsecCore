
namespace ParsecCore
{
    struct ParseError
    {
        public ParseError(string error, int lineNumber)
        {
            LineNumber = lineNumber;
            Error = error;
        }

        public int LineNumber { get; init; }

        public string Error { get; init; }

        public override string ToString()
        {
            return $"line {LineNumber}: {Error}";
        }
    }
}
