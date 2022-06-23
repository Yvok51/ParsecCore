namespace ParsecCore.Input
{
    class StringParserInput : IParserInput
    {
        public StringParserInput(string input)
        {
            _input = input;
            _position = Position.Start;
        }

        public bool EndOfInput => _position.Offset >= _input.Length;

        public Position Position => _position;

        public char Read()
        {
            char readChar = _input[_position.Offset];
            _position = readChar == '\n' ? _position.NextLine() : _position.NextColumn();
            return readChar;
        }

        public void Seek(Position position)
        {
            _position = position;
        }

        private readonly string _input;
        private Position _position;
    }
}
