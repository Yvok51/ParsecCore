using Xunit;

using ParsecCore;
using ParsecCore.Input;

namespace ParsecCoreTests
{
    public class TryParserTests
    {
        [Fact]
        public void ParsedAndInputConsumed()
        {
            var input = ParserInput.Create("12abc");
            var parser = Parsers.String("12").Try();
            var initialPosition = input.Position;

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal("12", result.Right);
            Assert.NotEqual(initialPosition, input.Position);
        }

        [Fact]
        public void FailWithNothingConsumed()
        {
            var input = ParserInput.Create("abc");
            var parser = Parsers.String("12").Try();
            var initialPosition = input.Position;

            var result = parser(input);

            Assert.True(result.HasLeft);
            Assert.Equal(initialPosition, input.Position);
        }

        [Fact]
        public void FailWithInputConsumed()
        {
            var input = ParserInput.Create("1abc");
            var parser = Parsers.String("12").Try();
            var initialPosition = input.Position;

            var result = parser(input);

            Assert.True(result.HasLeft);
            Assert.Equal(initialPosition, input.Position);
        }
    }
}
