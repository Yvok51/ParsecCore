using ParsecCore;
using ParsecCore.Input;
using Xunit;

namespace ParsecCoreTests
{
    public class EOFParserTests
    {
        [Fact]
        public void ParsesCorrectly()
        {
            IParserInput input = ParserInput.Create("");

            var result = Parsers.EOF(input);

            Assert.True(result.HasRight);
            Assert.True(input.EndOfInput);
        }

        [Fact]
        public void FailsCorrectly()
        {
            IParserInput input = ParserInput.Create("This is outrageous. It's unfair");

            var result = Parsers.EOF(input);

            Assert.True(result.HasLeft);
            Assert.False(input.EndOfInput);
        }
    }
}
