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
            var input = ParserInput.Create(inputString);
            var parser = Parsers.AnyChar;

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(expected, result.Result);
        }

        [Fact]
        public void FailsCorrectly()
        {
            var input = ParserInput.Create("");
            var parser = Parsers.AnyChar;

            var result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
