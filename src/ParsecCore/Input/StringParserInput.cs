using System;

namespace ParsecCore.Input
{
    internal sealed class StringParserInput : IParserInput<char>
    {
        public StringParserInput(string input, Position position, Func<char, Position, Position> updatePosition)
        {
            _input = input;
            _updatePosition = updatePosition;
            _position = position;
            EndOfInput = _position.Offset >= _input.Length; // we can cache result, since input is immutable
        }

        public StringParserInput(string input, Func<char, Position, Position> updatePosition)
            : this(input, Position.Start(), updatePosition)
        {
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
            EndOfInput = _position.Offset >= _input.Length; // we can cache result, since input is immutable
        }

        public bool EndOfInput { get; init; }

        public Position Position => _position;

        public IParserInput<char> Advance()
        {
            return new StringParserInput(_input, _updatePosition(Current(), _position), _updatePosition);
        }

        public char Current()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }

            return _input[_position.Offset];
        }

        public bool Equals(IParserInput<char>? other)
        {
            return other is not null && Position.Offset == other.Position.Offset; // Presume we are not mixing inputs
        }

        public override bool Equals(object? obj)
        {
            return obj is StringParserInput other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_input, _position, _updatePosition);
        }

        private readonly Func<char, Position, Position> _updatePosition;
        private readonly string _input;
        private readonly Position _position;
    }
}
