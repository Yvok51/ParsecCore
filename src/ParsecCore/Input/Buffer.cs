using System.IO;
using System.Text;

namespace ParsecCore.Input
{
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

        public char Read(long offset)
        {
            if (offset >= _offsetOfFirstChar && offset < _offsetOfFirstChar + BUFFER_SIZE)
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
            _reader.BaseStream.Position = startingOffset;
            _reader.DiscardBufferedData();
            _offsetOfFirstChar = startingOffset;
            _lastReadChars = _reader.Read(_buffer, 0, BUFFER_SIZE);
        }

        public bool EndOfStream(long offset)
        {
            // presumes that we only advance forward by one and also that the given offset follows this
            return _offsetOfFirstChar + _lastReadChars <= offset;
        }

        private static readonly int BUFFER_SIZE = 4096;
        private int _lastReadChars;
        private long _offsetOfFirstChar;
        private readonly char[] _buffer;
        private readonly StreamReader _reader;
    }
}
