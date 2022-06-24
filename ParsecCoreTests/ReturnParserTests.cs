using Xunit;

using ParsecCore;
using ParsecCore.Input;

namespace ParsecCoreTests
{
    public class ReturnParserTests
    {
        [Fact]
        public void ValueReturned()
        {
            IParserInput input = ParserInput.Create("Do you know the tragedy of...");
            IParser<int> parser = Parser.Return(100);

            var result = parser.Parse(input);

            Assert.True(result.HasRight);
            Assert.Equal(100, result.Right);
        }

        [Fact]
        public void InputNotConsumed()
        {
            IParserInput input = ParserInput.Create("not just the men, but the women and the children, too");
            IParser<int> parser = Parser.Return(100);

            var _ = parser.Parse(input);

            Assert.Equal(0, input.Position.Offset);
        }
    }
}
