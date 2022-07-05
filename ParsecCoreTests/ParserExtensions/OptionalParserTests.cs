using ParsecCore;
using ParsecCore.Input;
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

            Assert.True(result.HasRight);
            Assert.False(result.Right.IsEmpty);
            Assert.Equal("12", result.Right.Value);
        }

        [Fact]
        public void NothingParsed()
        {
            var input = ParserInput.Create("abc");
            var parser = Parsers.String("12").Optional();

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.True(result.Right.IsEmpty);
        }

        [Fact]
        public void FailIfInputConsumed()
        {
            var input = ParserInput.Create("1abc");
            var parser = Parsers.String("12").Optional();

            var result = parser(input);

            Assert.True(result.HasLeft);
        }
    }
}
