using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ParsecCore.Input
{
    public class ParserInput
    {
        /// <summary>
        /// Creates an input for the parser from a string
        /// </summary>
        /// <param name="inputTokens"> The string which will serve as input </param>
        /// <param name="tabSize">
        /// The size of the tab in the input, by default 4 spaces.
        /// Used in specifying the location in the input for error messages.
        /// </param>
        /// <returns> Input for a parser </returns>
        /// <exception cref="ArgumentNullException"> If input string is null </exception>
        public static IParserInput<char> Create(string inputTokens, int tabSize = 4)
        {
            if (inputTokens is null) throw new ArgumentNullException(nameof(inputTokens));

            return new StringParserInput(inputTokens, tabSize);
        }

        /// <summary>
        /// Creates an input for the parser from a string
        /// </summary>
        /// <param name="inputTokens"> The string which will serve as input </param>
        /// <param name="updatePosition">
        /// The function the string uses to calculate the next location in the input.
        /// Be aware that <see cref="Position"/> specifies the line and column in the input,
        /// as well as the offset in the input string we are located at.
        /// </param>
        /// <returns> Input for a parser </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static IParserInput<char> Create(string inputTokens, Func<char, Position, Position> updatePosition)
        {
            if (inputTokens is null) throw new ArgumentNullException(nameof(inputTokens));
            if (updatePosition is null) throw new ArgumentNullException(nameof(updatePosition));

            return new StringParserInput(inputTokens, updatePosition);
        }

        /// <summary>
        /// Creates an input for the parser from a list of tokens
        /// </summary>
        /// <param name="inputTokens"> The tokens which will serve as input </param>
        /// <param name="updatePosition">
        /// The function the string uses to calculate the next location in the input.
        /// Be aware that <see cref="Position"/> specifies the line and column in the input,
        /// as well as the offset in the input list we are located at.
        /// </param>
        /// <typeparam name="T"> The type of tokens </typeparam>
        /// <returns> Input for a parser </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static IParserInput<T> Create<T>(IReadOnlyList<T> inputTokens, Func<T, Position, Position> updatePosition)
        {
            if (inputTokens is null) throw new ArgumentNullException(nameof(inputTokens));
            if (updatePosition is null) throw new ArgumentNullException(nameof(updatePosition));

            return new TokenListParserInput<T>(inputTokens, updatePosition);
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
        /// Creates an input for the parser from a stream.     
        /// The underlying stream must be readable and seekable.
        /// If the stream does not satisfy these condition, an <see cref="ArgumentException"/> is thrown
        /// The stream is not disposed of, the caller still holds the ownership of the stream.
        /// </summary>
        /// <param name="stream"> The stream from which the input will read </param>
        /// <param name="encoding"> The encoding of the input </param>
        /// <param name="updatePosition">
        /// The function the string uses to calculate the next location in the stream.
        /// Be aware that <see cref="Position"/> specifies the line and column in the input,
        /// as well as the offset in the input stream we are located at.
        /// </param>
        /// <returns> Input for a parser </returns>
        /// <exception cref="ArgumentNullException"> If any argument is null </exception>
        /// <exception cref="ArgumentException"> If the stream is not readable and seekable </exception>
        public static IParserInput<char> Create(
            Stream stream,
            Encoding encoding,
            Func<char, Position, Position> updatePosition)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (encoding is null) throw new ArgumentNullException(nameof(encoding));
            if (updatePosition is null) throw new ArgumentNullException(nameof(updatePosition));

            return new StreamParserInput(stream, encoding, updatePosition);
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

        /// <summary>
        /// Creates an input for the parser from a stream.     
        /// The underlying stream must be readable and seekable.
        /// If the stream does not satisfy these condition, an <see cref="ArgumentException"/> is thrown
        /// The stream is not disposed of, the caller still holds the ownership of the stream.
        /// </summary>
        /// <param name="reader"> The reader the input will read from </param>
        /// <param name="updatePosition">
        /// The function the string uses to calculate the next location in the stream.
        /// Be aware that <see cref="Position"/> specifies the line and column in the input,
        /// as well as the offset in the input stream we are located at.
        /// </param>
        /// <returns> Input for a parser </returns>
        /// <exception cref="ArgumentNullException"> If any argument is null </exception>
        /// <exception cref="ArgumentException"> If the stream is not readable and seekable </exception>
        public static IParserInput<char> Create(StreamReader reader, Func<char, Position, Position> updatePosition)
        {
            if (reader is null) throw new ArgumentNullException(nameof(reader));
            if (updatePosition is null) throw new ArgumentNullException(nameof(updatePosition));

            return new StreamParserInput(reader, updatePosition);
        }
    }
}
