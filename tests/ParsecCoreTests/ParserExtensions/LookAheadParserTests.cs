using ParsecCore;
using ParsecCore.Input;
using Xunit;

namespace ParsecCoreTests
{
    public class LookAheadParserTests
    {
        [Fact]
        public void ParsedAndNothingConsumed()
        {
            var input = ParserInput.Create("12abc");
            var parser = Parsers.String("12").LookAhead();
            var initialPosition = input.Position;

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal("12", result.Result);
            Assert.Equal(initialPosition, input.Position);
        }

        [Fact]
        public void FailWithNothingConsumed()
        {
            var input = ParserInput.Create("abc");
            var parser = Parsers.String("12").LookAhead();
            var initialPosition = input.Position;

            var result = parser(input);

            Assert.True(result.IsError);
            Assert.Equal(initialPosition, input.Position);
        }

        [Fact]
        public void FailWithInputConsumed()
        {
            var input = ParserInput.Create("1abc");
            var parser = Parsers.String("12").LookAhead();
            var initialPosition = input.Position;

            var result = parser(input);

            Assert.True(result.IsError);
            Assert.NotEqual(initialPosition, result.UnconsumedInput.Position);
        }
    }
}
