using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace ParsecCore.Input
{
    /// <summary>
    /// Servers as input for parsing reading from an underlying stream.
    /// The underlying stream must be readable and seekable.
    /// The stream is not disposed of, the caller still holds the ownership of the stream.
    /// </summary>
    internal class StreamParserInput : IParserInput<char>
    {
        public StreamParserInput(StreamReader reader, int tabSize)
        {
            if (reader is null || !reader.BaseStream.CanRead || !reader.BaseStream.CanSeek)
            {
                throw new ArgumentException("Provided reader must be not null, must be able to read, and must be able to seek");
            }

            _reader = new Buffer(reader);
            _position = Position.Start((int)reader.BaseStream.Position);
            _updatePosition = DefaultUpdatePosition(tabSize, reader.CurrentEncoding);
        }

        public StreamParserInput(StreamReader reader, Func<char, Position, Position> updatePosition)
        {
            if (reader is null || !reader.BaseStream.CanRead || !reader.BaseStream.CanSeek)
            {
                throw new ArgumentException("Provided reader must be not null, must be able to read, and must be able to seek");
            }

            _reader = new Buffer(reader);
            _position = Position.Start((int)reader.BaseStream.Position);
            _updatePosition = updatePosition;
        }

        public StreamParserInput(Stream stream, Encoding encoding, int tabSize)
        {
            if (stream is null || !stream.CanRead || !stream.CanSeek)
            {
                throw new ArgumentException("Provided stream must be not null, must be able to read, and must be able to seek");
            }

            _reader = new Buffer(stream, encoding);
            _position = Position.Start((int)stream.Position);
            _updatePosition = DefaultUpdatePosition(tabSize, encoding);
        }

        public StreamParserInput(Stream stream, Encoding encoding, Func<char, Position, Position> updatePosition)
        {
            if (stream is null || !stream.CanRead || !stream.CanSeek)
            {
                throw new ArgumentException("Provided stream must be not null, must be able to read, and must be able to seek");
            }

            _reader = new Buffer(stream, encoding);
            _position = Position.Start((int)stream.Position);
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

        public bool EndOfInput => _reader.EndOfStream;

        public Position Position => _position;

        public char Read()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }

            char readChar = _reader.Read();
            _position = _updatePosition(readChar, _position);
            return readChar;
        }

        public void Seek(Position position)
        {
            if (position.Offset != _position.Offset)
            {
                _reader.Seek(position.Offset);
            }
            _position = position;
        }

        public char Peek()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }

            return _reader.Peek();
        }

        private readonly Func<char, Position, Position> _updatePosition;
        private readonly Buffer _reader;
        private Position _position;
    }
}
