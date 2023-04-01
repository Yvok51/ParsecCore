using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsecCore.Input
{
    internal class TokenListParserInput<T> : IParserInput<T>
    {
        public TokenListParserInput(IReadOnlyList<T> input, Func<T, Position, Position> updatePosition)
        {
            _input = input;
            _updatePosition = updatePosition;
            _position = Position.Start();
        }

        public bool EndOfInput => _position.Offset >= _input.Count;

        public Position Position => _position;

        public T Read()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }
            T readToken = _input[_position.Offset];
            _position = _updatePosition(readToken, _position);
            return readToken;
        }

        public void Seek(Position position)
        {
            _position = position;
        }

        public T Peek()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }

            return _input[_position.Offset];
        }

        private readonly Func<T, Position, Position> _updatePosition;
        private readonly IReadOnlyList<T> _input;
        private Position _position;
    }
}
