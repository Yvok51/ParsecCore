using System;

namespace ParsecCore.Input
{
    internal class StringParserInput : IParserInput<char>
    {
        public StringParserInput(string input, int tabSize)
        {
            _input = input;
            _tabSize = tabSize;
            _position = Position.Start();
        }

        public bool EndOfInput => _position.Offset >= _input.Length;

        public Position Position => _position;

        public char Read()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }
            char readChar = _input[_position.Offset];
            UpdatePosition(readChar);
            return readChar;
        }

        private void UpdatePosition(char readChar)
        {
            _position = readChar switch
            {
                '\n' => _position.WithNewLine().WithIncreasedOffset(),
                '\t' => _position.WithTab(_tabSize).WithIncreasedOffset(),
                _ => _position.WithIncreasedColumn().WithIncreasedOffset()
            };
        }

        public void Seek(Position position)
        {
            _position = position;
        }

        public char Peek()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }

            return _input[_position.Offset];
        }

        private readonly int _tabSize;
        private readonly string _input;
        private Position _position;
    }
}
