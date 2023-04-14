using System;

namespace ParsecCore.Input
{
    /// <summary>
    /// Interface for parser inputs.
    /// The interface is designed to be immutable and that is also expected of the implementing classes.
    /// </summary>
    /// <typeparam name="T"> The type of the input symbols </typeparam>
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
        /// Answers whether the input has ended
        /// </summary>
        public bool EndOfInput { get; }

        /// <summary>
        /// The current position, see <see cref="Input.Position"/>
        /// </summary>
        public Position Position { get; }
    }
}
