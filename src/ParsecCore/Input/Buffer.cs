﻿using System.IO;
using System.Text;

namespace ParsecCore.Input
{
    /// <summary>
    /// Buffer over an input stream
    /// </summary>
    internal class Buffer
    {
        public Buffer(StreamReader reader)
        {
            _reader = reader;
            _buffer = new char[BUFFER_SIZE];
            _lastReadChars = int.MinValue;
            _offsetOfFirstChar = reader.BaseStream.Position;
            BufferNextData(_offsetOfFirstChar);
        }
        public Buffer(Stream stream, Encoding encoding) : this(new StreamReader(stream, encoding))
        {
        }

        /// <summary>
        /// Read a character at a specified offset.
        /// If the offset is not in the buffer, new data is buffered starting in the read position.
        /// </summary>
        /// <param name="offset"> The offset to read the character from </param>
        /// <returns> The read character </returns>
        public char Read(long offset)
        {
            if (offset >= _offsetOfFirstChar && offset < _offsetOfFirstChar + _lastReadChars)
            {
                return _buffer[offset - _offsetOfFirstChar];
            }
            else
            {
                BufferNextData(offset);
                return _buffer[0];
            }
        }

        private void BufferNextData(long startingOffset)
        {
            if (_reader.BaseStream.Position != startingOffset)
            {
                _reader.BaseStream.Position = startingOffset;
                _reader.DiscardBufferedData();
            }
            _offsetOfFirstChar = startingOffset;
            _lastReadChars = _reader.Read(_buffer, 0, BUFFER_SIZE);
        }

        /// <summary>
        /// Answers whether the given <paramref name="offset"/> is beyond the input stream.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool EndOfStream(long offset)
        {
            // presumes that we only advance forward by max one position at a time
            // and also that the given offset follows this pattern
            return _offsetOfFirstChar + _lastReadChars <= offset;
        }

        private static readonly int BUFFER_SIZE = 4096;
        private int _lastReadChars;
        private long _offsetOfFirstChar;
        private readonly char[] _buffer;
        private readonly StreamReader _reader;
    }
}
