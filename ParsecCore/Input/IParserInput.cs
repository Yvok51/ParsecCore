
namespace ParsecCore.Input
{
    interface IParserInput
    {
        public char Read();

        public void Seek(Position position);

        /// <summary>
        /// Answers whether the input has ended
        /// </summary>
        public bool EndOfInput { get; }

        public Position Position { get; }
    }
}
