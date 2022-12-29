using System;
using System.IO;
using System.Text;

namespace ParsecCore.Input
{
    public class ParserInput
    {
        /// <summary>
        /// Creates an input for the parser from a string
        /// </summary>
        /// <param name="inputString"> The string which will serve as input </param>
        /// <param name="tabSize">
        /// The size of the tab in the input, by default 4 spaces. Used in error messages
        /// </param>
        /// <returns> Input for a parser </returns>
        /// <exception cref="ArgumentNullException"> If input string is null </exception>
        public static IParserInput<char> Create(string inputString, int tabSize = 4)
        {
            if (inputString is null) throw new ArgumentNullException(nameof(inputString));

            return new StringParserInput(inputString, tabSize);
        }

        /// <summary>
        /// Creates an input for the parser from a stream.     
        /// The underlying stream must be readable and seekable.
        /// If the stream does not satisfy these condition, an <see cref="ArgumentException"/> is thrown
        /// The stream is not disposed of, the caller still holds the ownership of the stream.
        /// </summary>
        /// <param name="stream"> The stream from which the input will read </param>
        /// <param name="encoding"> The encoding of the input </param>
        /// <param name="tabSize">
        /// The size of the tab in the input, by default 4 spaces. Used in error messages
        /// </param>
        /// <returns> Input for a parser </returns>
        /// <exception cref="ArgumentNullException"> If any argument is null </exception>
        /// <exception cref="ArgumentException"> If the stream is not readable and seekable </exception>
        public static IParserInput<char> Create(Stream stream, Encoding encoding, int tabSize = 4)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (encoding is null) throw new ArgumentNullException(nameof(encoding));

            return new StreamParserInput(stream, encoding, tabSize);
        }

        /// <summary>
        /// Creates an input for the parser from a stream
        /// The underlying stream must be readable and seekable. 
        /// If the stream does not satisfy these condition, an <see cref="ArgumentException"/> is thrown
        /// Neither the reader or the underlying stream is disposed of, 
        /// the caller still holds the ownership of the stream.
        /// </summary>
        /// <param name="reader"> The reader the input will read from </param>
        /// <param name="tabSize">
        /// The size of the tab in the input, by default 4 spaces. Used in error messages
        /// </param>
        /// <returns> Input for a parser </returns>
        /// <exception cref="ArgumentNullException"> If reader is null </exception>
        /// <exception cref="ArgumentException"> If the reader is not readable and seekable </exception>
        public static IParserInput<char> Create(StreamReader reader, int tabSize = 4)
        {
            if (reader is null) throw new ArgumentNullException(nameof(reader));

            return new StreamParserInput(reader, tabSize);
        }
    }
}
