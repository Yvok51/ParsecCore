using System;

namespace ParsecCore.Input
{
    public interface IParserInput<T> : IEquatable<IParserInput<T>>
    {
        /// <summary>
        /// Reads and consumes a single symbol from the input.
        /// If a symbol cannot be read (e.g. end of input),
        /// then an <see cref="InvalidOperationException"/> is thrown
        /// </summary>
        /// <returns> The read character </returns>
        public IParserInput<T> Advance();

        /// <summary>
        /// Reads a single symbol from the input but does not consume it.
        /// If a symbol cannot be read (e.g. end of input),
        /// then an <see cref="InvalidOperationException"/> is thrown
        /// </summary>
        /// <returns> The read character </returns>
        public T Current();

        /// <summary>
        /// Seeks to the given position. 
        /// Whether the position is whithin the bound of the input is not checked until the next attempt to read
        /// </summary>
        /// <param name="position"> Position to seek to </param>
        // public void Seek(Position position);

        /// <summary>
        /// Answers whether the input has ended
        /// </summary>
        public bool EndOfInput { get; }

        /// <summary>
        /// The current position
        /// </summary>
        public Position Position { get; }
    }
}
