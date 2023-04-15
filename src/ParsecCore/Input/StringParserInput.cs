using System;

namespace ParsecCore.Input
{
    internal sealed class StringParserInput : IParserInput<char>
    {
        public StringParserInput(string input, Position position, Func<char, Position, Position> updatePosition)
            : this(input, position, updatePosition, 0)
        {
        }

        public StringParserInput(string input, Func<char, Position, Position> updatePosition)
            : this(input, Position.Start, updatePosition)
        {
        }

        public StringParserInput(string input, int tabSize)
            : this(input, Position.Start, DefaultUpdatePosition(tabSize))
        {
        }

        private StringParserInput(string input, Position position, Func<char, Position, Position> updatePosition, int offset)
        {
            _input = input;
            _updatePosition = updatePosition;
            _position = position;
            _offset = offset;
            EndOfInput = offset >= _input.Length; // we can cache result, since input is immutable
        }

        private static Func<char, Position, Position> DefaultUpdatePosition(int tabSize)
        {
            return (readChar, position) =>
            {
                return readChar switch
                {
                    '\n' => position.WithNewLine(),
                    '\t' => position.WithTab(tabSize),
                    _ => position.WithIncreasedColumn()
                };
            };
        }

        public bool EndOfInput { get; init; }

        public Position Position => _position;

        public int Offset => _offset;

        public IParserInput<char> Advance()
        {
            return new StringParserInput(_input, _updatePosition(Current(), _position), _updatePosition, _offset + 1);
        }

        public char Current()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }

            return _input[_offset];
        }

        public bool Equals(IParserInput<char>? other)
        {
            return other is not null && _offset == other.Offset; // Presume we are not mixing inputs
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
        private readonly int _offset;
    }
}
