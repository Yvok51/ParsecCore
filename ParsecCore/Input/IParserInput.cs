namespace ParsecCore.Input
{
    public interface IParserInput<out T>
    {
        /// <summary>
        /// Read and consume a single character from the input.
        /// </summary>
        /// <returns> The read character </returns>
        public T Read();

        /// <summary>
        /// Read a single character from the input but don't consume it
        /// </summary>
        /// <returns> The read character </returns>
        public T Peek();

        /// <summary>
        /// Seek to the given position
        /// </summary>
        /// <param name="position"> Position to seek to </param>
        public void Seek(Position position);

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
