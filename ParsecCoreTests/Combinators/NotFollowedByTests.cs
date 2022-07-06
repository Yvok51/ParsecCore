using ParsecCore;
using ParsecCore.Input;
using Xunit;

namespace ParsecCoreTests
{
    public class NotFollowedByTests
    {
        [Fact]
        public void ParseAndFail()
        {
            var input = ParserInput.Create("12abc");
            var parser = Combinators.NotFollowedBy(Parsers.String("12"), "12 not allowed");
            var initialPosition = input.Position;

            var result = parser(input);

            Assert.True(result.IsError);
            Assert.Equal(initialPosition, input.Position);
        }

        [Fact]
        public void DontParseWithNothingConsumed()
        {
            var input = ParserInput.Create("abc");
            var parser = Combinators.NotFollowedBy(Parsers.String("12"), "12 not allowed");
            var initialPosition = input.Position;

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(initialPosition, input.Position);
        }

        [Fact]
        public void DontParseWithInputConsumed()
        {
            var input = ParserInput.Create("1abc");
            var parser = Combinators.NotFollowedBy(Parsers.String("12"), "12 not allowed");
            var initialPosition = input.Position;

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(initialPosition, input.Position);
        }
    }
}
