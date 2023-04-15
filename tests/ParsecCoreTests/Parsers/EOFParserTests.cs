using ParsecCore;
using Xunit;

namespace ParsecCoreTests
{
    public class EOFParserTests
    {
        [Fact]
        public void ParsesCorrectly()
        {
            var input = ParserInput.Create("");

            var result = Parsers.EOF<char>()(input);

            Assert.True(result.IsResult);
            Assert.True(input.EndOfInput);
        }

        [Fact]
        public void FailsCorrectly()
        {
            var input = ParserInput.Create("This is outrageous. It's unfair");

            var result = Parsers.EOF<char>()(input);

            Assert.True(result.IsError);
            Assert.False(input.EndOfInput);
        }
    }
}
