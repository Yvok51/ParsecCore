namespace ParsecCore.Input
{
    public interface IParserInput
    {
        public char Read();

        public char Peek();

        public void Seek(Position position);

        /// <summary>
        /// Answers whether the input has ended
        /// </summary>
        public bool EndOfInput { get; }

        public Position Position { get; }
    }
}
