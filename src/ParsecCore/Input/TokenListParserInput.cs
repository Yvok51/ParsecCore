using System;
using System.Collections.Generic;

namespace ParsecCore.Input
{
    /// <summary>
    /// Implementation of a parser input (<see cref="IParserInput{T}"/>) for in lists of any symbols.
    /// </summary>
    internal sealed class TokenListParserInput<T> : IParserInput<T>
    {
        public TokenListParserInput(IReadOnlyList<T> input, Position position, Func<T, Position, Position> updatePosition)
            : this(input, position, updatePosition, 0)
        {
        }

        public TokenListParserInput(IReadOnlyList<T> input, Func<T, Position, Position> updatePosition)
            : this(input, Position.Start, updatePosition)
        {
        }

        private TokenListParserInput(IReadOnlyList<T> input, Position position, Func<T, Position, Position> updatePosition, int offset)
        {
            _input = input;
            _updatePosition = updatePosition;
            _position = position;
            _offset = offset;
            EndOfInput = offset >= _input.Count; // we can cache result, since input is immutable
        }

        public bool EndOfInput { get; init; }

        public Position Position => _position;

        public int Offset => _offset;

        public IParserInput<T> Advance()
        {
            return new TokenListParserInput<T>(_input, _updatePosition(Current(), _position), _updatePosition, _offset + 1);
        }

        public T Current()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }

            return _input[_offset];
        }

        public bool Equals(IParserInput<T>? other)
        {
            return other is not null && _offset == other.Offset; // Presume we are not mixing inputs
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
        private readonly int _offset;
    }
}
