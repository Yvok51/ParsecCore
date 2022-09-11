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
        public static IParserInput<char> Create(string inputString, int tabSize = 4)
            => new StringParserInput(inputString, tabSize);

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
        public static IParserInput<char> Create(Stream stream, Encoding encoding, int tabSize = 4)
            => new StreamParserInput(stream, encoding, tabSize);

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
        public static IParserInput<char> Create(StreamReader reader, int tabSize = 4)
            => new StreamParserInput(reader, tabSize);
    }
}
