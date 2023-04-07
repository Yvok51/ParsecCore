using System;
using System.Collections.Generic;

namespace ParsecCore.Input
{
    internal sealed class TokenListParserInput<T> : IParserInput<T>
    {
        public TokenListParserInput(IReadOnlyList<T> input, Position position, Func<T, Position, Position> updatePosition)
        {
            _input = input;
            _updatePosition = updatePosition;
            _position = position;
        }

        public TokenListParserInput(IReadOnlyList<T> input, Func<T, Position, Position> updatePosition)
            : this(input, Position.Start(), updatePosition)
        {
        }

        public bool EndOfInput => _position.Offset >= _input.Count;

        public Position Position => _position;

        public IParserInput<T> Advance()
        {
            return new TokenListParserInput<T>(_input, _updatePosition(Current(), _position), _updatePosition);
        }

        public T Current()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }

            return _input[_position.Offset];
        }

        public bool Equals(IParserInput<T>? other)
        {
            return other is not null && Position == other.Position; // Presume we are not mixing inputs
        }

        public override bool Equals(object? obj)
        {
            return obj is TokenListParserInput<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_input, _position, _updatePosition);
        }

        private readonly Func<T, Position, Position> _updatePosition;
        private readonly IReadOnlyList<T> _input;
        private readonly Position _position;
    }
}
