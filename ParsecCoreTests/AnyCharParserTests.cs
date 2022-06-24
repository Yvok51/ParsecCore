using Xunit;

using ParsecCore;
using ParsecCore.Input;

namespace ParsecCoreTests
{
    public class AnyCharParserTests
    {
        [Theory]
        [InlineData("now", 'n')]
        [InlineData("this", 't')]
        [InlineData("is", 'i')]
        [InlineData("podracing", 'p')]
        public void ParsesCorrectly(string inputString, char expected)
        {
            IParserInput input = ParserInput.Create(inputString);
            IParser<char> parser = Parser.AnyChar;

            var result = parser.Parse(input);

            Assert.True(result.HasRight);
            Assert.Equal(expected, result.Right);
        }

        [Fact]
        public void FailsCorrectly()
        {
            IParserInput input = ParserInput.Create("");
            IParser<char> parser = Parser.AnyChar;

            var result = parser.Parse(input);

            Assert.True(result.HasLeft);
        }
    }
}
