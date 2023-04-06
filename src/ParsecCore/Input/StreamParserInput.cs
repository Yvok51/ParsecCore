using System;
using System.IO;
using System.Text;

namespace ParsecCore.Input
{
    /// <summary>
    /// Servers as input for parsing reading from an underlying stream.
    /// The underlying stream must be readable and seekable.
    /// The stream is not disposed of, the caller still holds the ownership of the stream.
    /// </summary>
    internal sealed class StreamParserInput : IParserInput<char>
    {
        public StreamParserInput(StreamReader reader, int tabSize)
        {
            if (reader is null || !reader.BaseStream.CanRead || !reader.BaseStream.CanSeek)
            {
                throw new ArgumentException("Provided reader must be not null, must be able to read, and must be able to seek");
            }
            
            _position = Position.Start((int)reader.BaseStream.Position); // has to be before creation of Buffer
                                                                         // as it reads in constructor
            _reader = new Buffer(reader);
            _updatePosition = DefaultUpdatePosition(tabSize, reader.CurrentEncoding);
        }

        public StreamParserInput(StreamReader reader, Func<char, Position, Position> updatePosition)
        {
            if (reader is null || !reader.BaseStream.CanRead || !reader.BaseStream.CanSeek)
            {
                throw new ArgumentException("Provided reader must be not null, must be able to read, and must be able to seek");
            }

            _position = Position.Start((int)reader.BaseStream.Position); // has to be before creation of Buffer
                                                                         // as it reads in constructor
            _reader = new Buffer(reader);
            _updatePosition = updatePosition;
        }

        public StreamParserInput(Stream stream, Encoding encoding, int tabSize)
        {
            if (stream is null || !stream.CanRead || !stream.CanSeek)
            {
                throw new ArgumentException("Provided stream must be not null, must be able to read, and must be able to seek");
            }

            _position = Position.Start((int)stream.Position); // has to be before creation of Buffer
                                                              // as it reads in constructor
            _reader = new Buffer(stream, encoding);
            _updatePosition = DefaultUpdatePosition(tabSize, encoding);
        }

        public StreamParserInput(Stream stream, Encoding encoding, Func<char, Position, Position> updatePosition)
        {
            if (stream is null || !stream.CanRead || !stream.CanSeek)
            {
                throw new ArgumentException("Provided stream must be not null, must be able to read, and must be able to seek");
            }

            _position = Position.Start((int)stream.Position); // has to be before creation of Buffer
                                                              // as it reads in constructor
            _reader = new Buffer(stream, encoding);
            _updatePosition = updatePosition;
        }

        private StreamParserInput(Buffer reader, Position position, Func<char, Position, Position> updatePosition)
        {
            _reader = reader;
            _position = position;
            _updatePosition = updatePosition;
        }

        /// <summary>
        /// Updates the position based on the read character
        /// </summary>
        /// <param name="readChar"> The read character </param>
        private static Func<char, Position, Position> DefaultUpdatePosition(int tabSize, Encoding encoding)
        {
            return (readChar, position) =>
            {
                int offsetBy = encoding.GetByteCount(new char[] { readChar });
                return readChar switch
                {
                    '\n' => position.WithNewLine().WithIncreasedOffset(offsetBy),
                    '\t' => position.WithTab(tabSize).WithIncreasedOffset(offsetBy),
                    _ => position.WithIncreasedColumn().WithIncreasedOffset(offsetBy)
                };
            };
        }

        public bool EndOfInput => _reader.EndOfStream(_position.Offset);

        public Position Position => _position;

        public IParserInput<char> Advance()
        {
            return new StreamParserInput(_reader, _updatePosition(Current(), _position), _updatePosition);
        }

        public char Current()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }

            return _reader.Read(_position.Offset);
        }

        public bool Equals(IParserInput<char>? other)
        {
            return other is StreamParserInput otherInput
                && _reader.Equals(otherInput._reader) && Position == otherInput._position;
        }

        public override bool Equals(object? obj)
        {
            return obj is StringParserInput other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_reader, _position, _updatePosition);
        }

        private readonly Func<char, Position, Position> _updatePosition;
        private readonly Buffer _reader;
        private readonly Position _position;
    }
}
