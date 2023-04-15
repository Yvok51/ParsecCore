using ParsecCore;
using Xunit;

namespace ParsecCoreTests
{
    public class OptionalParserTests
    {
        [Fact]
        public void OptionalParsed()
        {
            var input = ParserInput.Create("12abc");
            var parser = Parsers.String("12").Optional();

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.False(result.Result.IsEmpty);
            Assert.Equal("12", result.Result.Value);
        }

        [Fact]
        public void NothingParsed()
        {
            var input = ParserInput.Create("abc");
            var parser = Parsers.String("12").Optional();

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result.IsEmpty);
        }

        [Fact]
        public void FailIfInputConsumed()
        {
            var input = ParserInput.Create("1abc");
            var parser = Parsers.String("12").Optional();

            var result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
