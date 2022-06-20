
namespace ParsecCore.Input
{
    interface IParserInput
    {
        public char Read();

        /// <summary>
        /// Answers whether the input has ended
        /// </summary>
        public bool EndOfInput { get; }

        public Position Position { get; }
    }
}
