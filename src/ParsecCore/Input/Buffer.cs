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
            _index = BUFFER_SIZE;
            _lastReadChars = int.MinValue;
            _offsetOfFirstChar = reader.BaseStream.Position;
        }
        public Buffer(Stream stream, Encoding encoding) : this(new StreamReader(stream, encoding))
        {
        }

        public char Peek()
        {
            if (_index < BUFFER_SIZE)
            {
                return _buffer[_index];
            }

            BufferNextData();
            return _buffer[_index];
        }

        public char Read()
        {
            if (_index < BUFFER_SIZE)
            {
                return _buffer[_index++];
            }

            BufferNextData();
            return _buffer[_index++];
        }

        public void Seek(long offset)
        {
            if (offset >= _offsetOfFirstChar && offset < _offsetOfFirstChar + BUFFER_SIZE)
            {
                _index = (int)(offset - _offsetOfFirstChar);
            }
            else
            {
                _reader.BaseStream.Position = offset;
                _reader.DiscardBufferedData();
                BufferNextData();
            }
        }

        private void BufferNextData()
        {
            _offsetOfFirstChar = _reader.BaseStream.Position;
            _lastReadChars = _reader.Read(_buffer, 0, BUFFER_SIZE);
            _index = 0;
        }

        public bool EndOfStream { get => _lastReadChars == _index; }

        private static readonly int BUFFER_SIZE = 4096;
        private int _lastReadChars;
        private int _index;
        private long _offsetOfFirstChar;
        private readonly char[] _buffer;
        private readonly StreamReader _reader;
    }
}
