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
        /// <summary>
        /// Create a <see cref="StreamParserInput"/>
        /// </summary>
        /// <param name="reader"> The <see cref="StreamReader"/> to use </param>
        /// <param name="updatePosition"> The update position function to use </param>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        /// <exception cref="ArgumentException"> If the reader is not readable or seekable </exception>
        public StreamParserInput(StreamReader reader, Func<char, Position, Position> updatePosition)
        {
            if (reader is null) throw new ArgumentNullException(nameof(reader));
            if (reader.BaseStream is null) throw new ArgumentNullException(nameof(reader));
            if (updatePosition is null) throw new ArgumentNullException(nameof(updatePosition));
            if (!reader.BaseStream.CanRead || !reader.BaseStream.CanSeek)
            {
                throw new ArgumentException("Provided reader must be able to read, and must be able to seek");
            }

            _position = Position.Start((int)reader.BaseStream.Position); // has to be before creation of Buffer
                                                                         // as it reads in constructor
            _reader = new Buffer(reader);
            _updatePosition = updatePosition;
            EndOfInput = _reader.EndOfStream(_position.Offset); // cache result, since input is immutable
        }

        /// <summary>
        /// Create a <see cref="StreamParserInput"/>
        /// </summary>
        /// <param name="reader"> The <see cref="StreamReader"/> to use </param>
        /// <param name="tabSize"> The size of a tab, used for updating the <see cref="Position"/> in the input </param>
        /// <exception cref="ArgumentNullException"> If <paramref name="reader"/> is null </exception>
        /// <exception cref="ArgumentException"> If the reader is not readable or seekable </exception>
        public StreamParserInput(StreamReader reader, int tabSize)
            : this(reader, DefaultUpdatePosition(tabSize, reader.CurrentEncoding))
        {
        }

        public StreamParserInput(Stream stream, Encoding encoding, Func<char, Position, Position> updatePosition)
            : this(new StreamReader(stream, encoding), updatePosition)
        {
        }

        public StreamParserInput(Stream stream, Encoding encoding, int tabSize)
            : this(stream, encoding, DefaultUpdatePosition(tabSize, encoding))
        {
        }

        private StreamParserInput(Buffer reader, Position position, Func<char, Position, Position> updatePosition)
        {
            _reader = reader;
            _position = position;
            _updatePosition = updatePosition;
            EndOfInput = _reader.EndOfStream(_position.Offset);
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

        public bool EndOfInput { get; init; }

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
            return other is not null && Position.Offset == other.Position.Offset; // Presume we are not mixing inputs
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
