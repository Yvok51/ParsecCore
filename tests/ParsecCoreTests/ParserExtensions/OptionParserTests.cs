using ParsecCore;
using Xunit;

namespace ParsecCoreTests
{
    public class OptionParserTests
    {
        [Fact]
        public void OptionParsed()
        {
            var input = ParserInput.Create("12abc");
            var parser = Parsers.String("12").Option("5");

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal("12", result.Result);
        }

        [Fact]
        public void NothingParsed()
        {
            var input = ParserInput.Create("abc");
            var parser = Parsers.String("12").Option("5");

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal("5", result.Result);
        }

        [Fact]
        public void FailIfInputConsumed()
        {
            var input = ParserInput.Create("1abc");
            var parser = Parsers.String("12").Option("5");

            var result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
