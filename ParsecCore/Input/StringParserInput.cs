using System;

namespace ParsecCore.Input
{
    internal class StringParserInput : IParserInput<char>
    {
        public StringParserInput(string input, Func<char, Position, Position> updatePosition)
        {
            _input = input;
            _updatePosition = updatePosition;
            _position = Position.Start();
        }

        public StringParserInput(string input, int _tabSize)
        {
            _input = input;
            _updatePosition = (readChar, position) =>
            {
                return readChar switch
                {
                    '\n' => position.WithNewLine().WithIncreasedOffset(),
                    '\t' => position.WithTab(_tabSize).WithIncreasedOffset(),
                    _ => position.WithIncreasedColumn().WithIncreasedOffset()
                };
            };
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
            _position = _updatePosition(readChar, _position);
            return readChar;
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

        private readonly Func<char, Position, Position> _updatePosition;
        private readonly string _input;
        private Position _position;
    }
}
