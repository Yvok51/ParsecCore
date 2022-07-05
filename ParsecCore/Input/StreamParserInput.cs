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
    class StreamParserInput : IParserInput
    {
        public StreamParserInput(StreamReader reader)
        {
            if (reader is null || !reader.BaseStream.CanRead || !reader.BaseStream.CanSeek)
            {
                throw new ArgumentException("Provided reader must be not null, must be able to read, and must be able to seek");
            }

            _reader = reader;
            _position = Position.Start((int)_reader.BaseStream.Position);
            _encoding = reader.CurrentEncoding;
        }

        public StreamParserInput(Stream stream, Encoding encoding)
        {
            if (stream is null || !stream.CanRead || !stream.CanSeek)
            {
                throw new ArgumentException("Provided stream must be not null, must be able to read, and must be able to seek");
            }

            _reader = new StreamReader(stream, encoding);
            _position = Position.Start((int)_reader.BaseStream.Position);
            _encoding = encoding;
        }
        public bool EndOfInput => _reader.EndOfStream;

        public Position Position => _position;

        public char Read()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }

            char readChar = (char)_reader.Read();
            int offsetBy = _encoding.GetByteCount(new char[] { readChar });
            _position = readChar == '\n' ? _position.NextLine(offsetBy) : _position.NextColumn(offsetBy);
            return readChar;
        }

        public void Seek(Position position)
        {
            if (position != _position)
            {
                _reader.BaseStream.Position = position.Offset;
                _reader.DiscardBufferedData();
                _position = position;
            }
        }

        public char Peek()
        {
            if (EndOfInput)
            {
                throw new InvalidOperationException("Read past the end of the input");
            }

            return (char)_reader.Peek();
        }

        private StreamReader _reader;
        private Encoding _encoding;
        private Position _position;
    }
}
