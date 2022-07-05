using ParsecCore;
using ParsecCore.Input;
using Xunit;

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
            Parser<char> parser = Parsers.AnyChar;

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(expected, result.Right);
        }

        [Fact]
        public void FailsCorrectly()
        {
            IParserInput input = ParserInput.Create("");
            Parser<char> parser = Parsers.AnyChar;

            var result = parser(input);

            Assert.True(result.HasLeft);
        }
    }
}
